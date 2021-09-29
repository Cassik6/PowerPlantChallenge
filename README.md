# PowerPlantChallenge
Engie Test Project 



# Who

Opap's Ditudidi
Satellit Junior .NET Developper


# What 

## Challenge Engie:

In the context of a challenge  designing a power production calculator. 
Exposing a Web API that receives a payload (containing a list of powerplant and energy prices) and returns the optimal power production,
in the form of a list of 

# How






## Return entity

## powerproduction
contains the name of the power plant and how much power it has to generate.

| field name   |      type      |  
|----------|:-------------:|
|name  |  string |
|p |    float   |


Power Production: {
    "name": string,
    "p": float
    }


### Input entity


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
}```
