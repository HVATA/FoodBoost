using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
using System.Net.Mail;

namespace RuokaAPI.Services
{
    public class Salasananlahetys
    {

        public string Uusisalasana { get; set; }



        public async Task<string> LahetaSalasana(string Sahkopostiosoite)
        {
            string FromMail = "foodboostx@gmail.com";
            string sovellussalasana = "pmtbjpjdhaavzixx";

            Guid guid = Guid.NewGuid();
            Uusisalasana = guid.ToString();

            try
            {

                MailMessage message = new MailMessage();
                message.From = new MailAddress(FromMail);
                message.Subject = "Salasanan palautus";
                message.To.Add(new MailAddress(Sahkopostiosoite));
                message.Body = "<html><body> <p> Hei, olit pyytänyt uutta salasanaa. </p> " +
                    "<p> Uusi salasanasi  on: " + Uusisalasana + "</p>" +
                    "<p> Voit käydä muuttamassa salasanasi saadulla salasanalla sivuston päivitä tietojasi kohdasta.</p>" +
                    "<p> Ystavällisin terveisin FoodBoost tiimi.</p> <body><html>";
                message.IsBodyHtml = true;

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(FromMail, sovellussalasana),
                    EnableSsl = true

                };
                smtpClient.Send(message);

                return Uusisalasana;

            }
            catch (Exception ex)
            {

                return "Lähetys ei onnistunut";

            }

        }



    }

    public class ReseptinLaheys
    {

        public string Sahkopostiosoite { get; set; }

        public string Otsikko { get; set; }

        public string reseptiString { get; set; }

        public string kuvaString { get; set; }



        public static bool OnKelvollinenBase64Kuva(string base64String)
        {
            try
            {
                //  Poistetaan "data:image/jpeg;base64," -alkuosa, jos se löytyy
                if (base64String.StartsWith("data:image"))
                {
                    base64String = base64String.Substring(base64String.IndexOf("base64,") + 7);
                }

                //  Poistetaan rivinvaihdot ja välilyönnit
                base64String = base64String.Replace("\n", "").Replace("\r", "").Replace(" ", "").Trim();

                //  Tarkistetaan, että Base64-pituus on jaollinen neljällä
                while (base64String.Length % 4 != 0)
                {
                    base64String += "=";
                }

                //  Muunnetaan Base64 string binääriksi
                byte[] kuvaBytes = Convert.FromBase64String(base64String);

                //  Tallennetaan väliaikainen kuva (tämä todistaa, että Base64 on oikein)
                string tempFile = Path.Combine(Path.GetTempPath(), "testikuva.jpg");
                File.WriteAllBytes(tempFile, kuvaBytes);

                Console.WriteLine($"✅ Base64 on kelvollinen, tallennettu: {tempFile}");
                return true;
            }
            catch (FormatException fe)
            {
                Console.WriteLine($" Base64 EI ole kelvollinen kuva: {fe.Message}");
                return false;
            }
        }




        public async Task<bool> LahetaResepti(string email, string otsikko, string reseptis, string kuvaString)
        {
            string FromMail = "foodboostx@gmail.com";
            string sovellussalasana = "pmtbjpjdhaavzixx";

            try
            {
                // 🔹 **Tarkistetaan Base64 ennen lähetystä**
                if (!OnKelvollinenBase64Kuva(kuvaString))
                {
                    Console.WriteLine("Base64 ei ole kelvollinen, sähköpostia ei lähetetä.");
                    return false;
                }

                MailMessage message = new MailMessage
                {
                    From = new MailAddress(FromMail),
                    Subject = otsikko,
                    IsBodyHtml = true
                };
                message.To.Add(new MailAddress(email));

                string kuvaHtml = "";

                if (!string.IsNullOrEmpty(kuvaString))
                {
                    try
                    {
                        byte[] kuvaBytes = Convert.FromBase64String(kuvaString);
                        MemoryStream ms = new MemoryStream(kuvaBytes);

                        var linkedResource = new LinkedResource(ms, "image/jpeg")
                        {
                            ContentId = "ReseptiKuva",
                            TransferEncoding = System.Net.Mime.TransferEncoding.Base64
                        };

                        kuvaHtml = "<p><img src='cid:ReseptiKuva' width='300'/></p>";

                        AlternateView htmlView = AlternateView.CreateAlternateViewFromString($@"
                <html>
                <body>
                    <p>Hei, olet saanut jaetun reseptin.</p>
                    <p>{reseptis}</p>
                    {kuvaHtml}
                    <p>Powered by FoodBoost</p>
                </body>
                </html>", null, "text/html");

                        htmlView.LinkedResources.Add(linkedResource);
                        message.AlternateViews.Add(htmlView);
                    }
                    catch (FormatException fe)
                    {
                        Console.WriteLine("Virhe Base64-muunnoksessa: " + fe.Message);
                        return false;
                    }
                }

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(FromMail, sovellussalasana),
                    EnableSsl = true
                };

                await smtpClient.SendMailAsync(message);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Virhe: " + ex.Message);
                return false;
            }
        }

    }
}