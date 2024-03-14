using System.ComponentModel.DataAnnotations;

namespace Etammen.ViewModels
{
    public class SerachViewModel
    {
        [Display(Name ="Specialty")]
        public List<string> Specialties { get;}= new List<string>() {
           "ALL",
           "Dermatology (Skin)","Dentistry (Teeth)", "Psychiatry (Mental, Emotional or Behavioral Disorders)",
           "Pediatrics and New Born (Child)","Neurology (Brain & Nerves)",
           "Gynaecology and Infertility","Ear, Nose and Throat", "Cardiology and Vascular Disease (Heart)",
           "Allergy and Immunology (Sensitivity and Immunity)","Andrology and Male Infertility","Audiology",
           "Cardiology and Thoracic Surgery (Heart & Chest)","Chest and Respiratory","Diabetes and Endocrinology",
          "Diagnostic Radiology (Scan Centers)","Dietitian and Nutrition","Family Medicine","Gastroenterology and Endoscopy",
            "General Practice",
            "General Surgery",
            "Geriatrics (Old People Health)",
            "Hematology",
            "Hepatology (Liver Doctor)",
            "Internal Medicine",
            "Interventional Radiology (Interventional Radiology)",
            "IVF and Infertility",
            "Laboratories",
            "Nephrology",
            "Neurosurgery (Brain & Nerves Surgery)",
            "Obesity and Laparoscopic Surgery",
            "Oncology (Tumor)",
            "Oncology Surgery (Tumor Surgery)",
            "Ophthalmology (Eyes)",
            "Osteopathy (Osteopathic Medicine)",
            "Pain Management",
            "Pediatric Surgery",
            "Phoniatrics (Speech)",
            "Physiotherapy and Sport Injuries",
            "Plastic Surgery",
            "Rheumatology",
            "Spinal Surgery",
            "Urology (Urinary System)",
            "Vascular Surgery (Arteries and Vein Surgery)"
        };
		public Dictionary< string,List<string> >city_areaDict = new Dictionary<string, List<string>>{
            {"ALL", new List<string>(){"ALL"} },
            {"Cairo", new List<string> {
                                        "ALL",
                                        "Heliopolis",
                                        "Nasr City",
                                        "El-Maadi",
                                        "New Cairo",
                                        "Hadayek El-Kobba",
                                        "El-Obour City",
                                        "El-Manyal",
                                        "Shoubra",
                                        "West El-Balad",
                                        "10th of Ramadan",
                                        "Ain Shams",
                                        "Badr City",
                                        "Boulaq",
                                        "Dar El-Salam",
                                        "El Salam City",
                                        "El-Abbasia",
                                        "El-Kattameya",
                                        "El-Marg",
                                        "El-Matareya",
                                        "El-Mokattam",
                                        "El-Rehab",
                                        "El-Sayeda Zainab",
                                        "El-Shorouk",
                                        "El-Zaitoun",
                                        "El-Zamalek",
                                        "Hadayek El Maadi",
                                        "Hadayek Helwan",
                                        "Helwan",
                                        "Madinaty",
                                        "Masr El-Kadima",
                                        "Shoubra El-Khei"} },

            {"Giza" , new List<string>(){ 
                                        "ALL",
                                        "Dokki and Mohandessin",
                                        "6th of October",
                                        "El-Haram",
                                        "Faisal",
                                        "El-Sheikh Zayed",
                                        "Hadayek El-Ahram",
                                        "El-Agouza",
                                        "New Giza",
                                        "Boulaq El Dakrour",
                                        "El-Ayyat",
                                        "El-Badrashin",
                                        "El-Giza",
                                        "El-Hawamdeya",
                                        "Imbaba"
                                                        } },

            {"Alexandria", new List<string>(){
                                        "ALL",
                                        "Mahatet El-Raml",
                                        "Roshdy",
                                        "Smouha",
                                        "Sporting",
                                        "Sidy Gaber",
                                        "Loran",
                                        "Camp Caesar",
                                        "El-Ibrahimia",
                                        "Janaklees",
                                        "Abou Keir",
                                        "Bahary",
                                        "Bakos",
                                        "Borg el-Arab",
                                        "Bulkly",
                                        "Cleopatra",
                                        "El-Agamy",
                                        "El-Amreya"

                                                         } },
            {"Qalyubia", new List<string>() {
                                         "ALL",
                                         "El Qanater El Khayreya",
                                         "Shibin Al Qanater",
                                         "Banha",
                                         "Qalyoub"
                                                         } },

            {"Menoufia", new List<string>() {
                                        "ALL",
                                        "Quwaysna",
                                        "El Shohada",
                                        "Ashmun",
                                        "Shibin El-Kom",
                                        "Menouf",
                                        "Sadat City",
                                        "Birket El Sabea"
            }},
            {"Fayoum", new List<string>() {
                                        "ALL",
                                        "Ibsheway",
                                        "El-Fayoum city"
                                                         } }
        };
       
    }
}
