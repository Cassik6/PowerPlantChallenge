![](/src/img/thumbnail.jpg)

# PowerPlantChallenge
Engie Test Project 

# Who

Opap's Ditudidi <br>
Satellit Junior .NET Developper


# What 

## Challenge Engie:

In the context of a challenge  designing a power production calculator. 
Exposing a Web API that receives a payload (containing a list of powerplant and energy prices) and returns the optimal power production,
in the form of a list containing the used powerplants and their energy production.

# How

## build

## launch

## endpoint: retrieve production plan

### Allowed HTTP Methods
POST - Returns a list of power productions

### Resource Information
Authentication - None <br>
Response Format - JSON <br>
Response Object - [powerproduction](#powerproduction) <br>
API Version - 1.0 <br>
Resource URI - https://localhost:8888/productionplan <br>
Parameters  - [payload](#payload) <br>


Example Request
GET https://api.cardmarket.com/ws/v2.0/games


# Entities

## Return entity

### powerproduction
contains the name of the power plant and how much power it has to generate.

| field name   |      type      |  description|
|----------|:-------------:|------:|
|name  |  string |
|p |    float   |


### example

```
[
  {
    "name": "windpark1",
    "p": 75
  },
  {
    "name": "windpark2",
    "p": 18
  },
  {
    "name": "gasfiredbig1",
    "p": 200
  },
  {
    "name": "gasfiredbig1",
    "p": 0
  },
  {
    "name": "tj1",
    "p": 0
  },
  {
    "name": "tj2",
    "p": 0
  }
]
```


## Input entity

### payload

| field name   |      type      |  description|
|----------|:-------------:|------:|
|load  |  float | need load for this cycle|
|fuels |    [fuel](#fuel)   | list of fuel prices|
|powerplants |    [powerplant](#powerplant)   | list of availlable powerplants|


### fuel

| field name   |      type      |  description|
|----------|:-------------:|------:|
|gas(euro/MWh)  |  float | gas price|
|kerosine(euro/MWh) |    float   | kerosine price|
|co2(euro/ton) |    float   | cost of CO2 emission |
|wind(%) |    float   | wind intensity|

### powerplant

| field name   |      type      |  description|
|----------|:-------------:|------:|
|name  |  string | name of the powerplant|
|type |    string   |  type of power plant, can only be `turbojet`, `gasfired`or  `windturbine`|
|efficiency |    float   | power production efficiency |
|pmin |    float   | minimum amount of energy that has to be produced when the powerplant is turned on|
|pmax |    float   | maximum amount of energy the powerplant can produce|



example
```{
  "load": 480,
  "fuels":
  {
    "gas(euro/MWh)": 13.4,
    "kerosine(euro/MWh)": 50.8,
    "co2(euro/ton)": 20,
    "wind(%)": 60
  },
  "powerplants": [
    {
      "name": "gasfiredbig1",
      "type": "gasfired",
      "efficiency": 0.53,
      "pmin": 100,
      "pmax": 460
    },
    {
      "name": "gasfiredbig2",
      "type": "gasfired",
      "efficiency": 0.53,
      "pmin": 100,
      "pmax": 460
    },
    {
      "name": "gasfiredsomewhatsmaller",
      "type": "gasfired",
      "efficiency": 0.37,
      "pmin": 40,
      "pmax": 210
    },
    {
      "name": "tj1",
      "type": "turbojet",
      "efficiency": 0.3,
      "pmin": 0,
      "pmax": 16
    },
    {
      "name": "windpark1",
      "type": "windturbine",
      "efficiency": 1,
      "pmin": 0,
      "pmax": 150
    },
    {
      "name": "windpark2",
      "type": "windturbine",
      "efficiency": 1,
      "pmin": 0,
      "pmax": 36
    }
  ]
}
```
