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

            Console.WriteLine($"a private key{ a.ToString() }");


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

    }
}
