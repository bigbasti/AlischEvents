using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.Entity;
using AlischEvents.Web.Security;

namespace AlischEvents.Web.Models {
	public class AlischDBInitializer : CreateDatabaseIfNotExists<AlischDB> {

		private string startseite_content = @"
			<link href='http://fonts.googleapis.com/css?family=Raleway:100' rel='stylesheet' type='text/css' />
			<link href='http://fonts.googleapis.com/css?family=Yanone+Kaffeesatz:200,300,400,700' rel='stylesheet' type='text/css' />
			<div class='row'>
				<div class='col-md-12'>
					<div id='feature'>
						<div class='row'>
							<div class='col-sm-7'>
								<div class='carousel slide' data-ride='carousel' id='carousel-example-generic'>
									<div class='carousel-inner' role='listbox'>
										<div class='item active'>
											<img class='img-responsive active' src='/Content/slider/bild1.jpg' /></div>
										<div class='item'>
											<img class='img-responsive' src='/Content/slider/bild2.jpg' /></div>
										<div class='item'>
											<img class='img-responsive' src='/Content/slider/bild1.jpg' /></div>
										<div class='item'>
											<img class='img-responsive' src='/Content/slider/bild3.jpg' /></div>
										<div class='item'>
											<img class='img-responsive' src='/Content/slider/bild1.jpg' /></div>
										<div class='item'>
											<img class='img-responsive' src='/Content/slider/bild4.jpg' /></div>
									</div>
								</div>
							</div>
							<div class='col-sm-5 hidden-xs'>
								<div id='feature-desc'>
									<h1>
										Alisch Events steht f&uuml;r ein kreatives, innovatives und serviceorientiertes Team mit langj&auml;hriger Erfahrung.</h1>
									<p>
										Lassen Sie Ihre Betriebsfeier, Roadshow oder Kick-Off-Veranstaltung zu einem unvergesslichen Erlebnis werden und vertrauen Sie im Eventmarketing unserer Agentur.</p>
								</div>
							</div>
						</div>
						<div class='row'>
							<div class='col-sm-12 visible-xs-block' id='feature-desc-xs'>
								<h1>
									Alisch Events steht f&uuml;r ein kreatives, innovatives und serviceorientiertes Team mit langj&auml;hriger Erfahrung.</h1>
								<p>
									Lassen Sie Ihre Betriebsfeier, Roadshow oder Kick-Off-Veranstaltung zu einem unvergesslichen Erlebnis werden und vertrauen Sie im Eventmarketing unserer Agentur.</p>
							</div>
						</div>
					</div>
				</div>
			</div>
			";

		private string kontakt_content = @"
			<h3>Haben Sie Fragen, Anregungen oder w&uuml;nschen weitere Informationen?</h3>
			<p>Kein Problem, f&uuml;llen Sie alle Felder aus und wir werden uns schnellstm&ouml;glich bei Ihnen melden!</p>
			<form action='/Home/Kontakt' method='post' name='Kontakt'>
				<table align='left' border='0' cellpadding='1' cellspacing='1'>
					<tbody>
						<tr>
							<td>
								<input name='Formular' type='hidden' value='Kontakt' /></td>
						</tr>
						<tr>
							<td>
								Ihr Name:</td>
						</tr>
						<tr>
							<td>
								<input class='form-control' name='Name' type='text' /></td>
						</tr>
						<tr>
							<td>
								Ihre E-Mail Adresse:</td>
						</tr>
						<tr>
							<td>
								<input class='form-control' name='Email' type='text' /></td>
						</tr>
						<tr>
							<td>
								Betreff</td>
						</tr>
						<tr>
							<td>
								<input class='form-control' name='Betreff' type='text' /></td>
						</tr>
						<tr>
							<td>
								<textarea class='form-control' cols='54' name='Nachricht' rows='10'>Ihre Anfrage</textarea></td>
						</tr>
						<tr>
							<td>
								<input class='btn btn-default' name='submit' type='submit' value='Anfrage absenden' /></td>
						</tr>
					</tbody>
				</table>
			</form>
			<div class='clear'>
				&nbsp;</div>
			<p>Alle von Ihnen angegebenen Informationen dienen nur der Kontaktaufnahme und werden unter keinen Umst&auml;nden an Dritte weitergegeben.</p>";

		private string kontakt_erfolg_content = @"
			<h3>
				Vielen Dank f&uuml;r Ihre Anfrage</h3>
			<p>
				Die Nachricht ist in unseren System eingegangen und wir werden uns schnellstm&ouml;glich bei Ihnen melden.</p>
			<p>
				Sollte es doch dringend sein k&ouml;nnen Sie uns auch problemlos telefonisch erreichen. Weitere Kontaktinformationen finden Sie ebenfalls auf unserer <a href='/Home/Kontakt'>Kontaktseite</a>.</p>";

		private string gallery_login_content = @"
			<h3>
				Um Zugriff auf die Bilder der Veranstaltung zu erhalten, ben&ouml;tigen Sie einen Zugangscode.</h3>
			<p>
				Den Zugangscode m&uuml;ssten Sie auf der Veranstaltung von unserem Eventpersonal erhalten haben.</p>
			<form action='/Gallery/Login' method='post' name='Login'>
				<table align='left' border='0' cellpadding='1' cellspacing='1'>
					<tbody>
						<tr>
							<td>
								Ihr Zugangscode:</td>
						</tr>
						<tr>
							<td>
								<input name='Code' size='30' type='text' class='form-control' /></td>
						</tr>
						<tr>
							<td>
								<input name='submit' type='submit' value='Galerie öffnen' class='btn btn-default' /></td>
						</tr>
					</tbody>
				</table>
			</form>
			<div class='clear'>
				&nbsp;</div>
			<p>
				Falls Sie keinen Code erhalten haben oder diesen Verloren haben kontaktieren Sie uns einfach.</p>
			";


		private string gallery_error_content = @"
			<div class='row'>
			  <div class='col-md-12'>
				<span style='font-size:20px;'>Der gew&uuml;nschte Inhalt kann nicht angezeigt werden.</span>
			  </div>
			</div>
			<div class='row'>
			  <div class='col-md-3'>
				<img alt='' src='/Content/grafik/design/lock.png' style='width: 128px; height: 128px; ' />
			  </div>
			  <div class='col-md-9'>
				<p>Folgende Umst&auml;nde k&ouml;nnen zu dem&nbsp;Fehler gef&uuml;hrt haben:</p>
				<ul>
					<li>
						Die Adresse ist fehlerhaft</li>
					<li>
						Das Bild wurde entfernt</li>
					<li>
						Sie haben keinen g&uuml;ltigen Zugangscode&nbsp;eingegeben</li>
					<li>
						Der Zugangscode, den Sie benutzt haben ist in der Zwischenzeit abgelaufen</li>
				</ul>
			  </div>
			</div>
			<div class='row'>
			  <div class='col-md-12'>
				<p>Bitte &uuml;berpf&uuml;fen Sie, ob Sie den Zugangscode korrekt eingegeben haben, hierbei wird auf die Gro&szlig;- und Kleinschreibung geachtet.</p>
				<p>F&uuml;r weitere Fragen oder bei Problemen mit dem Zugang zu der Galerie k&ouml;nnen Sie sich jeder Zeit &uuml;ber das Kontaktformular an uns wenden.</p>
			  </div>
			</div>";

		protected override void Seed(AlischDB context) {


			//Menüeinträge-------------------------------------------------------

			LinkedList<Menu.MenuEntry> MenuEntries = new LinkedList<Menu.MenuEntry>();

			MenuEntries.AddLast(new Menu.MenuEntry() {
				MenuOrderID = 1,
				Position = 0,
				Title = "Startseite",
				URL = "/"
			});

			MenuEntries.AddLast(new Menu.MenuEntry() {
				MenuOrderID = 2,
				Position = 0,
				Title = "Kontakt",
				URL = "/Home/Kontakt"
			});

			MenuEntries.AddLast(new Menu.MenuEntry() {
				MenuOrderID = 3,
				Position = 0,
				Title = "Galerie",
				URL = "/Site/Show/6"
			});

			MenuEntries.AddLast(new Menu.MenuEntry() {
				MenuOrderID = 1,
				Position = 1,
				Title = "Impressum",
				URL = "/Site/Show/4"
			});

			MenuEntries.AddLast(new Menu.MenuEntry() {
				MenuOrderID = 2,
				Position = 1,
				Title = "AGBs",
				URL = "/Site/Show/5"
			});

			MenuEntries.AddLast(new Menu.MenuEntry()
			{
				MenuOrderID = 3,
				Position = 1,
				Title = "Newsletter",
				URL = "/Newsletter/Public"
			});

			context.BlogMenus.Add(new Menu.Menu() {
				MenuEntries = MenuEntries
			});

			//Benutzer------------------------------------------------------------

			context.SiteUsers.Add(new Admin.SiteUser() {
				Email = "bigbasti@gmail.com",
				Firstname = "Sebastian",
				Lastname = "Gross",
				Password = Hashing.GenerateMD5("testpass"),
				Password2 = Hashing.GenerateMD5("testpass"),
				Username = "bigbasti"
			});

			context.SiteUsers.Add(new Admin.SiteUser() {
				Email = "info@alisch-eventagentur.de",
				Firstname = "Nadine",
				Lastname = "Alisch",
				Password = Hashing.GenerateMD5("event"),
				Password2 = Hashing.GenerateMD5("event"),
				Username = "nalisch"
			});

			//Statische Seiten ---------------------------------------------------

			context.WebSites.Add(new Site.WebSite() {       //ID 1 --> Wichtig! muss 1 sein
				IsSpecialSite = true,
				IsStaticSite = true,
				MenuOrderID = -1,
				ShowOnMenu = true,
				SiteContent = startseite_content,
				SiteLabel = "Startseite",
				SiteTitle = "Startseite",
				SiteURL = "/"
			});

			context.WebSites.Add(new Site.WebSite() {       //ID 2
				IsSpecialSite = false,
				IsStaticSite = true,
				MenuOrderID = -1,
				ShowOnMenu = true,
				SiteContent = kontakt_content,
				SiteLabel = "Kontakt",
				SiteTitle = "Haben Sie Fragen?",
				SiteURL = "/Home/Kontakt"
			});

			context.WebSites.Add(new Site.WebSite() {       //ID 3
				IsSpecialSite = false,
				IsStaticSite = true,
				MenuOrderID = -1,
				ShowOnMenu = false,
				SiteContent = kontakt_erfolg_content,
				SiteLabel = "[AnfrageErfolgreich]",
				SiteTitle = "Ihre Anfrage ist unterwegs",
				SiteURL = "-"
			});


			context.WebSites.Add(new Site.WebSite() {       //ID 4
				IsSpecialSite = false,
				IsStaticSite = true,
				MenuOrderID = -1,
				ShowOnMenu = false,
				SiteContent = "-",
				SiteLabel = "Impressum",
				SiteTitle = "Impressum",
				SiteURL = ""
			});

			context.WebSites.Add(new Site.WebSite() {       //ID 5
				IsSpecialSite = false,
				IsStaticSite = true,
				MenuOrderID = -1,
				ShowOnMenu = false,
				SiteContent = "Kein Inhalt",
				SiteLabel = "AGBs",
				SiteTitle = "AGBs",
				SiteURL = ""
			});

			context.WebSites.Add(new Site.WebSite() {       //ID 6
				IsSpecialSite = false,
				IsStaticSite = true,
				MenuOrderID = -1,
				ShowOnMenu = true,
				SiteContent = gallery_login_content,
				SiteLabel = "Galerie",
				SiteTitle = "Zugang zur Galerie",
				SiteURL = ""
			});

			context.WebSites.Add(new Site.WebSite() {       //ID 7
				IsSpecialSite = false,
				IsStaticSite = true,
				MenuOrderID = -1,
				ShowOnMenu = false,
				SiteContent = gallery_error_content,
				SiteLabel = "Fehlerseite",
				SiteTitle = "Fehler beim Laden der Seite",
				SiteURL = ""
			});
		}
	}
}