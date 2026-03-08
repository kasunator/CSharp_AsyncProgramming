using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EllipticCurveDiffieHellmanKeyExchange
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
            star_ElipticalCurveDeffieHellman();
        }

        void star_ElipticalCurveDeffieHellman()
        {
            ECDiffieHellman  a = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);
            ECDiffieHellmanPublicKey aPubKey = a.PublicKey;

            // Party B
            ECDiffieHellman b = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);
            ECDiffieHellmanPublicKey bPubKey = b.PublicKey;



            // Each side derives the shared secret
            byte[] aKey = a.DeriveKeyMaterial(bPubKey);
            byte[] bKey = b.DeriveKeyMaterial(aPubKey);

            a.Dispose();
            b.Dispose();

            Console.WriteLine($"A Key: {Convert.ToBase64String(aKey)}");
            Console.WriteLine($"B Key: {Convert.ToBase64String(bKey)}");

            // They will match
            Console.WriteLine("Keys match: " + aKey.AsSpan().SequenceEqual(bKey));
        }


        static byte[] HkdfSha256(ReadOnlySpan<byte> ikm, ReadOnlySpan<byte> salt, ReadOnlySpan<byte> info, int length)
        {
            // HKDF-Extract
            byte[] prk;
            using (var hmac = new HMACSHA256(salt.Length == 0 ? new byte[32] : salt.ToArray()))
                prk = hmac.ComputeHash(ikm.ToArray());

            // HKDF-Expand
            byte[] okm = new byte[length];
            byte[] t = Array.Empty<byte>();
            int pos = 0, counter = 1;
            using var hmac = new HMACSHA256(prk);
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

    }
}
