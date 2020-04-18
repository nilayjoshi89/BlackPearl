using System.Collections.Generic;

namespace BlackPearl.Controls.Demo
{
    public class Person
    {
        public string Name { get; set; }
        public string Company { get; internal set; }
        public string City { get; internal set; }
        public string Zip { get; internal set; }
        public string Info
        {
            get => $"{Name} - {Company}({Zip})";
        }
    }

    public static class PersonDataProvider
    {
        //Data generated using https://www.generatedata.com/
        public static IEnumerable<Person> GetDummyData()
        {
            yield return new Person()
            {
                Name = "Shad L. Hernandez",
                Company = "Phasellus Elit Pede Corporation",
                City = "Genova",
                Zip = "24092"
            };
            yield return new Person()
            {
                Name = "Karly D. Howell",
                Company = "Lobortis Risus In Consulting",
                City = "Regina",
                Zip = "553940"
            };
            yield return new Person()
            {
                Name = "Erica O. Lamb",
                Company = "Lorem Institute",
                City = "Dera Bugti",
                Zip = "61473"
            };
            yield return new Person()
            {
                Name = "Acton C. Benson",
                Company = "Cras PC",
                City = "Dawson Creek",
                Zip = "22463"
            };
            yield return new Person()
            {
                Name = "Coby N. Carroll",
                Company = "Phasellus Associates",
                City = "Jodoigne-Souveraine",
                Zip = "83976"
            };
            yield return new Person()
            {
                Name = "Allen W. Sherman",
                Company = "Primis In LLC",
                City = "Oberwart",
                Zip = "372787"
            };
            yield return new Person()
            {
                Name = "Lucy K. Pennington",
                Company = "Auctor Vitae LLC",
                City = "Floridablanca",
                Zip = "05934"
            };
            yield return new Person()
            {
                Name = "Akeem R. Hester",
                Company = "Duis Mi Consulting",
                City = "Forge-Philippe",
                Zip = "88979"
            };
            yield return new Person()
            {
                Name = "Tyrone Q. Dillon",
                Company = "Condimentum Corporation",
                City = "Palencia",
                Zip = "70-060"
            };
            yield return new Person()
            {
                Name = "Lucius Y. Pollard",
                Company = "Erat LLC",
                City = "Laken",
                Zip = "Z4886"
            };
            yield return new Person()
            {
                Name = "Ori P. Small",
                Company = "Volutpat Nulla Corp.",
                City = "WanfercŽe-Baulet",
                Zip = "26721-157"
            };
            yield return new Person()
            {
                Name = "Shoshana J. Forbes",
                Company = "Magnis Dis Limited",
                City = "San Francisco",
                Zip = "6425"
            };
            yield return new Person()
            {
                Name = "Keefe J. Wolfe",
                Company = "Amet Consectetuer Adipiscing Associates",
                City = "Chihuahua",
                Zip = "773797"
            };
            yield return new Person()
            {
                Name = "Desiree A. Vazquez",
                Company = "Quis Pede Inc.",
                City = "Gloucester",
                Zip = "Z5202"
            };
            yield return new Person()
            {
                Name = "Olympia G. Gordon",
                Company = "Morbi Neque Tellus Incorporated",
                City = "Brandon",
                Zip = "71626-279"
            };
            yield return new Person()
            {
                Name = "Lareina N. Barker",
                Company = "Sed Sem Egestas Associates",
                City = "Kulti-Barakar",
                Zip = "3096"
            };
            yield return new Person()
            {
                Name = "Price H. Farrell",
                Company = "A Aliquet LLP",
                City = "Şereflikoçhisar",
                Zip = "44-912"
            };
            yield return new Person()
            {
                Name = "Devin S. Giles",
                Company = "Lectus Pede Ultrices Corporation",
                City = "Lota",
                Zip = "765836"
            };
            yield return new Person()
            {
                Name = "Bree I. Hall",
                Company = "Nec Company",
                City = "Bristol",
                Zip = "00457"
            };
            yield return new Person()
            {
                Name = "Vincent P. Pope",
                Company = "Arcu Vestibulum Limited",
                City = "Sale",
                Zip = "84872"
            };
            yield return new Person()
            {
                Name = "Grady V. Juarez",
                Company = "Adipiscing Elit Aliquam Ltd",
                City = "Santa Caterina Villarmosa",
                Zip = "434766"
            };
            yield return new Person()
            {
                Name = "Upton F. Robles",
                Company = "Ornare Placerat Orci LLP",
                City = "Hamme-Mille",
                Zip = "P0L 7Z2"
            };
            yield return new Person()
            {
                Name = "Zeph I. Paul",
                Company = "Donec LLC",
                City = "Bossuit",
                Zip = "20729"
            };
            yield return new Person()
            {
                Name = "Dana L. Short",
                Company = "Sit Amet Diam Industries",
                City = "Ashburton",
                Zip = "39-568"
            };
            yield return new Person()
            {
                Name = "Idona P. Strickland",
                Company = "In Ltd",
                City = "Vologda",
                Zip = "24802-156"
            };
            yield return new Person()
            {
                Name = "Jermaine P. Mckay",
                Company = "Quis Arcu Company",
                City = "Novoli",
                Zip = "1905"
            };
            yield return new Person()
            {
                Name = "Susan N. Simon",
                Company = "Sem Ut Incorporated",
                City = "Lac Ste. Anne",
                Zip = "68223"
            };
            yield return new Person()
            {
                Name = "Phyllis C. Warner",
                Company = "Magna Sed Eu Corporation",
                City = "Curitiba",
                Zip = "0967 AR"
            };
            yield return new Person()
            {
                Name = "Karleigh P. Pacheco",
                Company = "Proin Ultrices Consulting",
                City = "Scala Coeli",
                Zip = "7608 NF"
            };
            yield return new Person()
            {
                Name = "Venus H. Kramer",
                Company = "Mauris Institute",
                City = "Berlare",
                Zip = "A6X 9C4"
            };
            yield return new Person()
            {
                Name = "Baxter L. Mcfadden",
                Company = "Risus Donec Foundation",
                City = "Piancastagnaio",
                Zip = "96804"
            };
            yield return new Person()
            {
                Name = "Ivory J. Woodward",
                Company = "Arcu Vestibulum Ut Industries",
                City = "Gander",
                Zip = "13989"
            };
            yield return new Person()
            {
                Name = "Edward M. Rivera",
                Company = "Duis Risus Foundation",
                City = "Bevagna",
                Zip = "80764"
            };
            yield return new Person()
            {
                Name = "Ulric Y. Cross",
                Company = "Elit Elit Institute",
                City = "Tokoroa",
                Zip = "44913"
            };
            yield return new Person()
            {
                Name = "Sierra C. Mcneil",
                Company = "Rutrum Non Hendrerit Inc.",
                City = "Dégelis",
                Zip = "5509"
            };
            yield return new Person()
            {
                Name = "Melanie J. Mcdaniel",
                Company = "Egestas Foundation",
                City = "Randazzo",
                Zip = "434944"
            };
            yield return new Person()
            {
                Name = "Zenaida N. Palmer",
                Company = "Dolor Sit Amet Ltd",
                City = "San Esteban",
                Zip = "3330 EU"
            };
            yield return new Person()
            {
                Name = "Hop U. Quinn",
                Company = "Amet Risus Associates",
                City = "Dilbeek",
                Zip = "717686"
            };
            yield return new Person()
            {
                Name = "Gannon C. Carson",
                Company = "Arcu Eu Foundation",
                City = "Virelles",
                Zip = "423986"
            };
            yield return new Person()
            {
                Name = "Sonia L. Wong",
                Company = "Eu Nulla Ltd",
                City = "Vremde",
                Zip = "41115"
            };
            yield return new Person()
            {
                Name = "Genevieve K. Roy",
                Company = "Purus Mauris A Corp.",
                City = "Sindelfingen",
                Zip = "661253"
            };
            yield return new Person()
            {
                Name = "Isabella U. Bennett",
                Company = "Metus Vitae LLP",
                City = "Moerkerke",
                Zip = "2560"
            };
            yield return new Person()
            {
                Name = "Sandra X. Mcgee",
                Company = "Mattis LLC",
                City = "Poucet",
                Zip = "Z2989"
            };
            yield return new Person()
            {
                Name = "Colin S. Pratt",
                Company = "Nonummy Ipsum Non PC",
                City = "Palmiano",
                Zip = "5342"
            };
            yield return new Person()
            {
                Name = "Declan U. Wallace",
                Company = "Quis Urna Nunc Corp.",
                City = "Metairie",
                Zip = "4980"
            };
            yield return new Person()
            {
                Name = "Hammett Q. Morrow",
                Company = "Turpis Egestas Aliquam Incorporated",
                City = "Peine",
                Zip = "58651"
            };
            yield return new Person()
            {
                Name = "Charissa T. Pitts",
                Company = "Cras Company",
                City = "Clermont-Ferrand",
                Zip = "52602-19345"
            };
            yield return new Person()
            {
                Name = "Kelly V. Wise",
                Company = "Pellentesque Eget Dictum Inc.",
                City = "Bangor",
                Zip = "81766"
            };
            yield return new Person()
            {
                Name = "Sybil V. Solomon",
                Company = "Sodales Nisi Magna Incorporated",
                City = "Newport",
                Zip = "72134-45940"
            };
            yield return new Person()
            {
                Name = "Ryder K. Wilkerson",
                Company = "Orci Consectetuer Euismod Associates",
                City = "Norfolk",
                Zip = "2921 NZ"
            };
            yield return new Person()
            {
                Name = "Cairo U. Sampson",
                Company = "Urna Ut Tincidunt Industries",
                City = "Vrasene",
                Zip = "59-841"
            };
            yield return new Person()
            {
                Name = "Briar M. Rodriquez",
                Company = "Aliquet Molestie Ltd",
                City = "Belogorsk",
                Zip = "41415"
            };
            yield return new Person()
            {
                Name = "Burton U. Rosario",
                Company = "Metus Foundation",
                City = "Créteil",
                Zip = "335588"
            };
            yield return new Person()
            {
                Name = "Tucker O. Wyatt",
                Company = "Egestas LLP",
                City = "Rotheux-RimiŽre",
                Zip = "Z3414"
            };
            yield return new Person()
            {
                Name = "Kyra Y. Graves",
                Company = "Non Enim LLC",
                City = "StrŽe",
                Zip = "437277"
            };
            yield return new Person()
            {
                Name = "Francesca V. Blair",
                Company = "Convallis Convallis Company",
                City = "Monticelli d'Ongina",
                Zip = "223591"
            };
            yield return new Person()
            {
                Name = "Dawn B. Mcknight",
                Company = "Augue Ac LLC",
                City = "Lede",
                Zip = "14238"
            };
            yield return new Person()
            {
                Name = "Ignatius D. Gould",
                Company = "Eget Nisi Dictum LLC",
                City = "Rekem",
                Zip = "27015"
            };
            yield return new Person()
            {
                Name = "Kameko B. Oneil",
                Company = "Enim Gravida Sit Limited",
                City = "Campbeltown",
                Zip = "08868"
            };
            yield return new Person()
            {
                Name = "Aspen H. Horne",
                Company = "Eleifend Non Associates",
                City = "Wrexham",
                Zip = "9032"
            };
            yield return new Person()
            {
                Name = "Yeo N. Delacruz",
                Company = "Urna Nunc Corporation",
                City = "Williams Lake",
                Zip = "73881"
            };
            yield return new Person()
            {
                Name = "Damian R. Bush",
                Company = "Amet Ornare LLP",
                City = "Lourdes",
                Zip = "52977"
            };
            yield return new Person()
            {
                Name = "Denise C. Bowen",
                Company = "Nisl Arcu Iaculis Corporation",
                City = "Canoas",
                Zip = "3017"
            };
            yield return new Person()
            {
                Name = "Upton A. Odom",
                Company = "Ornare Sagittis Company",
                City = "Bama",
                Zip = "370076"
            };
            yield return new Person()
            {
                Name = "Olympia I. Carroll",
                Company = "Quisque Ornare Tortor Foundation",
                City = "Anyang",
                Zip = "34016"
            };
            yield return new Person()
            {
                Name = "Francesca K. Humphrey",
                Company = "Non Bibendum Sed Industries",
                City = "Sahiwal",
                Zip = "14315"
            };
            yield return new Person()
            {
                Name = "Edan K. Tanner",
                Company = "Id Nunc Incorporated",
                City = "Ichalkaranji",
                Zip = "251876"
            };
            yield return new Person()
            {
                Name = "Christine I. Lloyd",
                Company = "Mollis Inc.",
                City = "Castellina in Chianti",
                Zip = "86508"
            };
            yield return new Person()
            {
                Name = "Axel B. Campos",
                Company = "Cubilia Curae; Phasellus Limited",
                City = "Fontanellato",
                Zip = "206713"
            };
            yield return new Person()
            {
                Name = "Myles J. Cabrera",
                Company = "Sit Amet Lorem Foundation",
                City = "Sakhalin",
                Zip = "595954"
            };
            yield return new Person()
            {
                Name = "Natalie Y. Schneider",
                Company = "Ornare Industries",
                City = "Pointe-Claire",
                Zip = "77773"
            };
            yield return new Person()
            {
                Name = "Shellie L. Jones",
                Company = "Eros Non Enim Consulting",
                City = "Vladimir",
                Zip = "M65 6SU"
            };
            yield return new Person()
            {
                Name = "Caldwell D. Savage",
                Company = "Pellentesque A Industries",
                City = "Mitú",
                Zip = "96089"
            };
            yield return new Person()
            {
                Name = "Zeph H. Hayes",
                Company = "Non Lobortis Quis Incorporated",
                City = "Sivry",
                Zip = "80405"
            };
            yield return new Person()
            {
                Name = "Brock F. Turner",
                Company = "Vivamus Euismod Urna Corp.",
                City = "Lenna",
                Zip = "Y0M 9M4"
            };
            yield return new Person()
            {
                Name = "Macon P. Fletcher",
                Company = "Ullamcorper Nisl Arcu Industries",
                City = "Hagen",
                Zip = "01761"
            };
            yield return new Person()
            {
                Name = "Gregory Y. Wall",
                Company = "Enim Gravida Incorporated",
                City = "Volgograd",
                Zip = "60396"
            };
            yield return new Person()
            {
                Name = "Morgan K. Sellers",
                Company = "Mi Limited",
                City = "Oristano",
                Zip = "35191"
            };
            yield return new Person()
            {
                Name = "Ciaran V. Mcdowell",
                Company = "Id Incorporated",
                City = "Clearwater Municipal District",
                Zip = "79453-10499"
            };
            yield return new Person()
            {
                Name = "Inga A. Mcintosh",
                Company = "Tristique LLP",
                City = "Tielrode",
                Zip = "86477"
            };
            yield return new Person()
            {
                Name = "Wilma H. Mathews",
                Company = "Donec Non Justo Limited",
                City = "Levin",
                Zip = "690845"
            };
            yield return new Person()
            {
                Name = "Burke Q. Salinas",
                Company = "Ullamcorper Corp.",
                City = "Innisfail",
                Zip = "52786-938"
            };
            yield return new Person()
            {
                Name = "Rhona P. Conner",
                Company = "Sociis Natoque PC",
                City = "Felitto",
                Zip = "34872-42037"
            };
            yield return new Person()
            {
                Name = "Daryl Q. Gates",
                Company = "Nulla Aliquet Proin Ltd",
                City = "Minturno",
                Zip = "506805"
            };
            yield return new Person()
            {
                Name = "Otto E. Callahan",
                Company = "Suspendisse Company",
                City = "Galzignano Terme",
                Zip = "X6 3NG"
            };
            yield return new Person()
            {
                Name = "Destiny U. Mcmahon",
                Company = "Tristique Senectus Limited",
                City = "Gibsons",
                Zip = "95472-28390"
            };
            yield return new Person()
            {
                Name = "Austin S. Leblanc",
                Company = "Nascetur Corporation",
                City = "Waret-l'Evque",
                Zip = "09595"
            };
            yield return new Person()
            {
                Name = "Lunea Y. Mccormick",
                Company = "Auctor Quis Incorporated",
                City = "Treglio",
                Zip = "9903 RA"
            };
            yield return new Person()
            {
                Name = "Kennedy X. Mcleod",
                Company = "Congue Incorporated",
                City = "Parbhani",
                Zip = "O3 6YQ"
            };
            yield return new Person()
            {
                Name = "Daquan X. Shannon",
                Company = "Quam Pellentesque Associates",
                City = "Corbara",
                Zip = "31909"
            };
            yield return new Person()
            {
                Name = "Jescie L. Wilkerson",
                Company = "Erat In Inc.",
                City = "Thon",
                Zip = "40607"
            };
            yield return new Person()
            {
                Name = "Carol S. Christensen",
                Company = "In Mi Pede Inc.",
                City = "Metro",
                Zip = "UD08 7HU"
            };
            yield return new Person()
            {
                Name = "Giselle N. Wyatt",
                Company = "Vitae Semper Egestas Associates",
                City = "Rüsselsheim",
                Zip = "R9R 3OK"
            };
            yield return new Person()
            {
                Name = "Freya E. Perkins",
                Company = "Urna Corp.",
                City = "Jefferson City",
                Zip = "I1H 1JQ"
            };
            yield return new Person()
            {
                Name = "Dennis B. Mcfadden",
                Company = "Ultricies Adipiscing Enim Consulting",
                City = "Heerhugowaard",
                Zip = "3642"
            };
            yield return new Person()
            {
                Name = "Jessica A. Barrera",
                Company = "Nonummy Ac Company",
                City = "Greenock",
                Zip = "57149"
            };
            yield return new Person()
            {
                Name = "Rama F. Holland",
                Company = "Sagittis Placerat Industries",
                City = "Saumur",
                Zip = "99686-20759"
            };
            yield return new Person()
            {
                Name = "Daryl B. Holden",
                Company = "Ornare Facilisis Eget Consulting",
                City = "Neudörfl",
                Zip = "Z3040"
            };
            yield return new Person()
            {
                Name = "Charde L. Gentry",
                Company = "Massa LLP",
                City = "Coronel",
                Zip = "260337"
            };
            yield return new Person()
            {
                Name = "Maggy V. Gonzalez",
                Company = "Libero Company",
                City = "Bonefro",
                Zip = "11640-099"
            };
        }
    }
}
