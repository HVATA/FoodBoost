using System.Net;
using System.Net.Mail;

namespace RuokaAPI.Services
{
    public class Salasananlahetys
    {

        public string Uusisalasana { get; set; }

        

        public async Task<string>LahetaSalasana(string Sahkopostiosoite)
        {
            string FromMail = "foodboostx@gmail.com";
            string sovellussalasana = "pmtbjpjdhaavzixx";

            Guid guid = Guid.NewGuid();
            Uusisalasana = guid.ToString();

            MailMessage message = new MailMessage();
            message.From =new MailAddress(FromMail);
            message.Subject = "Salasanan palautus";
            message.To.Add(new MailAddress(Sahkopostiosoite));
            message.Body = "<html><body> <p> Hei, olit pyytänyt uutta salasanaa. </p> " +
                "<p> Uusi salasanasi  on: " +Uusisalasana +"</p>" +
                "<p> Voit käydä muuttamassa salasanasi saadulla salasanalla sivuston päivitä tietojasi kohdasta.</p>"+
                "<p> Ystavällisin terveisin FoodBoost tiimi.</p> <body><html>";
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(FromMail,sovellussalasana),
                EnableSsl = true

            };
            smtpClient.Send(message);

            return Uusisalasana;
        }



    }
}
