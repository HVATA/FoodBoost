using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
using System.Net.Mail;

namespace FoodAPI.Services
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



        public async Task<bool> LahetaResepti(string email, string otsikko, string reseptis, string base64KuvaString)
        {
            string FromMail = "foodboostx@gmail.com";
            string sovellussalasana = "pmtbjpjdhaavzixx";
            string kuvatiedostoPolku = null;

            try
            {
                // 🔹 Luodaan väliaikainen kuvatiedosto Base64-stringistä
                kuvatiedostoPolku = LuoValiaikainenKuvatiedosto(base64KuvaString);

                if (string.IsNullOrEmpty(kuvatiedostoPolku) || !File.Exists(kuvatiedostoPolku))
                {
                    Console.WriteLine("Kuvaa ei voitu luoda, sähköpostia ei lähetetä.");
                    return false;
                }

                // 🔹 Luodaan sähköpostiviesti
                MailMessage message = new MailMessage
                {
                    From = new MailAddress(FromMail),
                    Subject = otsikko,
                    IsBodyHtml = true
                };
                message.To.Add(new MailAddress(email));

                // 🔹 Luodaan Content-ID (CID) -viittaus kuvaan
                var linkedResource = new LinkedResource(kuvatiedostoPolku, "image/jpeg")
                {
                    ContentId = "ReseptiKuva",
                    TransferEncoding = System.Net.Mime.TransferEncoding.Base64
                };

                // 🔹 Rakennetaan HTML-viesti, jossa kuva upotettuna
                string htmlContent = $@"
        <html>
        <body>
            <p>Hei, olet saanut jaetun reseptin.</p>
            <p>{reseptis}</p>
            <p><img src='cid:ReseptiKuva' width='300'/></p>
            <p>Powered by FoodBoost</p>
        </body>
        </html>";

                // Luodaan viestin HTML-näkymä ja lisätään kuva
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlContent, null, "text/html");
                htmlView.LinkedResources.Add(linkedResource);
                message.AlternateViews.Add(htmlView);

                //Lähetetään sähköposti
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(FromMail, sovellussalasana),
                    EnableSsl = true
                };

                await smtpClient.SendMailAsync(message);

                Console.WriteLine("Sähköposti lähetetty onnistuneesti upotetulla kuvalla!");

                //Vapautetaan tiedosto ennen poistamista
                linkedResource.Dispose();
                message.Dispose();

                // Poistetaan väliaikainen tiedosto
                File.Delete(kuvatiedostoPolku);
                Console.WriteLine($"Väliaikainen tiedosto poistettu: {kuvatiedostoPolku}");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Virhe sähköpostin lähetyksessä: {ex.Message}");
                return false;
            }
        }

        public static string LuoValiaikainenKuvatiedosto(string base64String)
        {
            try
            {
                // Poistetaan mahdollinen "data:image/jpeg;base64," -alkuosa
                if (base64String.StartsWith("data:image"))
                {
                    base64String = base64String.Substring(base64String.IndexOf("base64,") + 7);
                }

                // Poistetaan rivinvaihdot ja ylimääräiset merkit
                base64String = base64String.Replace("\n", "").Replace("\r", "").Replace(" ", "").Trim();

                // Tarkistetaan, että Base64-pituus on jaollinen neljällä
                while (base64String.Length % 4 != 0)
                {
                    base64String += "=";
                }

                // Muunnetaan Base64-stringi binääriksi
                byte[] kuvaBytes = Convert.FromBase64String(base64String);

                // Luodaan väliaikainen tiedostopolku
                string tempFilePath = Path.Combine(Path.GetTempPath(), $"kuva_{Guid.NewGuid()}.jpg");

                // Tallennetaan tiedosto
                File.WriteAllBytes(tempFilePath, kuvaBytes);

                Console.WriteLine($"Väliaikainen kuvatiedosto luotu: {tempFilePath}");

                return tempFilePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Virhe kuvan luomisessa: {ex.Message}");
                return null;
            }
        }

    }
}
       