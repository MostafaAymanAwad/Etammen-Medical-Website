# Etammen Medical Website 
***
## a medical website to connect users and doctors.

### a breif about the funcionality of the website from users and doctors perspectives:

#### Etammen website is designed to help both users and doctors to communicate in an automated way to make it easier and time saving for user to book doctor appointments, and for doctor to reach out to help more people

#### 1- user:
- can search for doctors by city, medical speciality, doctor's degree, doctor's name or clinic's name
- can sort clinics and doctors by fees or rating
- can filter clinics and doctors by location, availablty, fees, gender of doctor, doctor's degree
- user can view clinics location on map
- can book appointment and pay at attendance or pay online using stripe payment gateway
- can request home visit if doctor accept home visits
- can check his appointment history or cancel un-attended appointments and get refunded if he paid online
- user can check all doctor's ratings and other user's comments
- user can review the doctor after attending the appointment

  
#### 2- doctor
- doctor can add his clinics and their details(name, location, fees, available booking days, opening and closing hours,...)
- doctor can manage his appointment and cancel appointment he can't take (also user will get notified by sms)
- can edit his profile and add all personal details (degree, speciality, certificates, ...)


#### communication between website, users, and doctors
- user recieve sms for appointment confirmation or cancellation
- doctor recieve updates once his documents are verified and he is a verified doctor on the website
- user recieve an email for confirming his identity and email registered on the website

#### admin Dashboard:
- admin approve doctors documents to be verified on the website
- admin manage all website entities 

***
### technical aspects:

### Authentication and Authorization (using Identity Asp.Net core):
- easy registeration flow for both users and doctors with server-side and client-side validations
- resetting password (forgot password) functionality by sending email using smtp or an sms using Twillio to user to reset his password
- external login provider support using google or facebook
- doctor can de-activate his account to not appear on user's searches, also he can re-activate it at any time

### structured Logging
- all request, error status, exception are handled and logged in json file and also in database for higher severity exceptions and errors


## technologies and tools used :
- c# 12
- ASP.Net Core MVC
- Asp.Net Core Identity
- MS Sql Server
- Entity Framework Core 8 + LINQ
- javascript
- Bootstrap 5
- Auto mapper
- Serilog structured logging
- Data tables
- integrating with stripe payment gateway
- integrating with google and facebook apis for external login
- integrating with Twillio for sms service 
- integrating with Gmail smtp for email service 
- integrating with Leaflet Maps for clinics location on map 


## concepts applied:
- Clean Architecture using 3 layers architecture (Data access layer - business logic layer - presentation layer)
- Unit Of Work Pattern along with repositories
- SOLID Principles    
- Seperation of concerns by mapping domain models and data alyer models and viewmodel to facilate communication between different layers


***
### all Source Code files and Database back-up and UML diagrams of project are included in repository.
***

### Project Team On Linked-In: 

#### Mohamed Ashraf:
  (https://www.linkedin.com/in/mohamed-ashraf-9961a022a/)

#### Mostafa Ayman:
  (https://www.linkedin.com/in/mostafa-ayman-/)

#### Mostafa Foad: 
  (https://www.linkedin.com/in/mostafa-foad/)

#### Moaz Samy:
  (https://www.linkedin.com/in/moaz-samy/)

#### Ahmed Fawzi: 
  (https://www.linkedin.com/in/ahmed-fawzi-136225269/)

***
##### Website demo video on all linked in accounts.

 
