using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Etammen.Helpers;

public class DoctorRegisterationHelper
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    public DoctorRegisterationHelper(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public List<string> SpecialitySelectList { get; private set; } = new List<string>()
        {
            "ALL",
            "Allergy and Immunology",
            "Anesthesiology",
            "Cardiology",
            "Dermatology",
            "Emergency Medicine",
            "Endocrinology",
            "Family Medicine",
            "Gastroenterology",
            "Geriatrics",
            "Hematology",
            "Infectious Diseases",
            "Internal Medicine",
            "Neonatology",
            "Nephrology",
            "Neurology",
            "Obstetrics and Gynecology",
            "Oncology",
            "Ophthalmology",
            "Orthopedics",
            "Otolaryngology",
            "Pathology",
            "Pediatrics",
            "Physical Medicine and Rehabilitation",
            "Plastic Surgery",
            "Psychiatry",
            "Pulmonology",
            "Radiology",
            "Rheumatology",
            "Sports Medicine",
            "Urology"
            };
          
    public SelectList DegreeSelectList { get; private set; } = new SelectList
    (
        new List<string>
        {
            "General Practitioner (GP)",
            "Professor",
            "Lecturer",
            "Specialist",
            "Consultant",
        }
    );


    public Dictionary<string, List<string>> CityAreasDictionary { get; private set; } = new Dictionary<string, List<string>>
    {
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

                                                         }},
            {"Qalyubia", new List<string>() {
                                         "ALL",
                                         "El Qanater El Khayreya",
                                         "Shibin Al Qanater",
                                         "Banha",
                                         "Qalyoub"
                                                         }},

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

    public List<string> SaveUploadedImages(List<IFormFile> uploadeImages,string ImagesFolderName)
    {
        List<string> imagesNames = new List<string>();
        foreach (IFormFile image in uploadeImages)
        {
            string DoctorImagesPath = Path.Combine(_webHostEnvironment.WebRootPath, ImagesFolderName);
            string UniqueFileName = $"{Guid.NewGuid().ToString()}_{image.FileName}";
            string imagePath = Path.Combine(DoctorImagesPath, UniqueFileName);
            imagesNames.Add(UniqueFileName);
    
            using(var stream = new FileStream(imagePath, FileMode.Create))
            {
                image.CopyTo(stream);
                stream.Close();
            }
        }
        return imagesNames;
    }
}
