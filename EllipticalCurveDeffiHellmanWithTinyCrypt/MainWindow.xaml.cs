using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Cryptography;

namespace EllipticalCurveDeffiHellmanWithTinyCrypt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Suppose you received TinyCrypt peer pubkey as 64-byte X||Y:
            byte[] peerXY64 = /* from device */;
            byte[] myPub;
            byte[] aesKey = EcdhWithTinyCrypt(peerXY64, out myPub, use65ByteForm: false);


        }

        static byte[] ExportUncompressed65(ECDiffieHellman ecdh)
        {
            var p = ecdh.ExportParameters(false);
            byte[] outBuf = new byte[65];
            outBuf[0] = 0x04;
            Buffer.BlockCopy(p.Q.X, 0, outBuf, 1, 32);
            Buffer.BlockCopy(p.Q.Y, 0, outBuf, 33, 32);
            return outBuf;
        }

        static byte[] ExportRawXY64(ECDiffieHellman ecdh)
        {
            var p = ecdh.ExportParameters(false);
            byte[] outBuf = new byte[64];
            Buffer.BlockCopy(p.Q.X, 0, outBuf, 0, 32);
            Buffer.BlockCopy(p.Q.Y, 0, outBuf, 32, 32);
            return outBuf;
        }

        static void SplitPeerXY(ReadOnlySpan<byte> peer, out byte[] x, out byte[] y)
        {
            if (peer.Length == 65 && peer[0] == 0x04)
            {
                x = peer.Slice(1, 32).ToArray();
                y = peer.Slice(33, 32).ToArray();
            }
            else if (peer.Length == 64)
            {
                x = peer.Slice(0, 32).ToArray();
                y = peer.Slice(32, 32).ToArray();
            }
            else
            {
                throw new ArgumentException("Peer key must be 64 bytes (X||Y) or 65 bytes (04||X||Y).");
            }
        }

        static ECDiffieHellmanPublicKey ImportPeerPublicKey(ReadOnlySpan<byte> peer)
        {
            SplitPeerXY(peer, out var x, out var y);
            var parms = new ECParameters
            {
                Curve = ECCurve.NamedCurves.nistP256,
                Q = new ECPoint { X = x, Y = y }
            };
            using var temp = ECDiffieHellman.Create(parms); // holds only the peer public
            return temp.PublicKey;
        }


        static byte[] HkdfSha256(ReadOnlySpan<byte> ikm, ReadOnlySpan<byte> salt, ReadOnlySpan<byte> info, int length)
        {
            // HKDF-Extract
            byte[] prk;
            using HMACSHA256 hmac = new HMACSHA256(salt.Length == 0 ? new byte[32] : salt.ToArray());
                prk = hmac.ComputeHash(ikm.ToArray());

            // HKDF-Expand
            byte[] okm = new byte[length];
            byte[] t = Array.Empty<byte>();
            int pos = 0, counter = 1;
            hmac = new HMACSHA256(prk);
            while (pos < length)
            {
                byte[] input = new byte[t.Length + info.Length + 1];
                Buffer.BlockCopy(t, 0, input, 0, t.Length);
                Buffer.BlockCopy(info.ToArray(), 0, input, t.Length, info.Length);
                input[^1] = (byte)counter++;

                t = hmac.ComputeHash(input);
                int take = Math.Min(t.Length, length - pos);
                Buffer.BlockCopy(t, 0, okm, pos, take);
                pos += take;
            }
            CryptographicOperations.ZeroMemory(prk);
            CryptographicOperations.ZeroMemory(t);
            return okm;
        }

        public static byte[] EcdhWithTinyCrypt(ReadOnlySpan<byte> peerPub, out byte[] myPubForPeer, bool use65ByteForm = true)
        {
            using var me = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);

            // Send my public key to peer
            myPubForPeer = use65ByteForm ? ExportUncompressed65(me) : ExportRawXY64(me);

            // Build peer public key
            using var peer = ImportPeerPublicKey(peerPub);

            // Raw ECDH shared secret Z (x-coordinate per standards)
            byte[] z = me.DeriveKeyMaterial(peer);

            // Derive a symmetric key (e.g., 32-byte AES-256 key) via HKDF-SHA256
            // Choose salt/info that BOTH sides agree on
            byte[] salt = Array.Empty<byte>(); // or a per-session/random salt that both sides share
            byte[] info = System.Text.Encoding.ASCII.GetBytes("TinyCrypt-P256-ECDH AES-256 key");
            byte[] aesKey = HkdfSha256(z, salt, info, 32);

            CryptographicOperations.ZeroMemory(z);
            return aesKey;
        }

    }
}