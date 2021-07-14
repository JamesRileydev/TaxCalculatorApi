# TaxCalculator API

The application is written in .NET 5.0 using C# 9 and Visual Studio 2019 Community Edition \
This application will accept a JSON array of objects and return the item totals, sales taxes, and total for the order in JSON format


## Installation
Ensure the .NET 5.0 SDK is installed on your machine \
Download the repository and open the solution with Visual Studio 2019 \
Build the solution \
\
The repository contains a "Files" directory which contains all of the required sample input requests, as well as requests to test validation


## Example Usage
---
***NOTE***

#####  The "category" is an ENUM and the options are as follows:
Tax Exempt = 1 \
Tax Exempt Import = 2  (5%)\
Basic Tax = 3  (10%)\
Basic and Import Tax = 4 (5% + 10%)

---

#### Using Insomia or Postman:
URL: http://localhost:9090/api/orders \
POST request
```Json
[
	{
	"name": "Book",
	"price" : 12.49,
	"category" :  1
    }
]
```
#### Using CURL:
```Curl
curl --request POST \
  --url http://localhost:9090/api/orders \
  --header 'Content-Type: application/json' \
  --data '[
	{
	"name": "Book",
	"price" : 12.49,
	"category" :  1
	}
]' 
```
#### Example Response:
```Json
{
    "Id": "12b7c040-787c-4e6f-9df1-e225acf60a48",
    "OrderItems": [
        "Book: 12.49"
    ],
    "SalesTaxes": "0.00",
    "Total": "12.49"
}
