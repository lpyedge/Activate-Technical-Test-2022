# Run the project in source code
* Use Visual Studio 2022 to open the Bakery.sln file in the root directory;
* Select the project Bakery.Site and run it;
* Open http://localhost:5050 with your browser.
* The project will generate test data when it starts for the first time.

# Docker way to run the project
* Start the command line and go to the project Bakery.Site directory (containing the Dokerfile file);
* Run the command docker build -f Dockerfile -t bakerysite/test . && docker run -p 5050:80 --name bakerysite_test bakerysite/test
* Open http://localhost:5050 with your browser
* Test data will be generated when the project starts for the first time


## Product display interface
You can view the product list and product details

You can communicate with Bob to order products through the email button at the bottom right corner of the display screen (requires customer's computer to have email app)


### Quickly switch the template of the product display interface to see the display effect:
http://localhost:5050/?template=1  
http://localhost:5050/?template=2



## The login address for the admin backend
http://localhost:5050/admin

Account: Bob  
Password: Bob's_Bakery

Please pay attention to the letter case, the account password does not contain spaces


## You can change the basic information of the website in the admin backend:
Site Name
Website description
Website Logo
Customer Service Email
Website template (currently there are two sets of templates)


## Management background can achieve the operation of the product:
Products can be temporarily hidden and not displayed on the display page
Commodities can be freely uploaded and modified icons
Commodity can set the price and cost to facilitate later sales bookkeeping


## important data file list:

System configuration file  
/App_Data/Config.json

Sqlite database file  
/App_Data/Data.db

Uploaded files directory  
/Upload