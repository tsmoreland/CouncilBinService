[![REST DTO CI Build](https://github.com/tsmoreland/CouncilBinService/actions/workflows/rest-dto.yml/badge.svg)](https://github.com/tsmoreland/CouncilBinService/actions/workflows/rest-dto.yml)

# Council Bin Service
Various applications with querying bin schedule from ArsBorough webservice - more of an adapter and chance to build a simple webapi with a vague purpose

## External

- external soap service used to retrieve data

## WebServiceFacade

- facade to the external soap service translating the respones to types the rest of the app can work with

## Bin Collections

- context responsible for handling collection queries, currently for a given address but eventually more generic identifiers

## Address 

- context responsible for translating addresses to something the webservice can work with providing an identifier that bin collection service can use
