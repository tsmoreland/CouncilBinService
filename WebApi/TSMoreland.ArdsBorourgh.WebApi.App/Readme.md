# Bin Collection App

REST API app providing collection dates by bin type for specified addresses by postcode and house number

Initially backed by data from the Ardsborough website but should rely primarily on cached that may periodically update or
require a manual refresh.

## Rest Endpoints

```
GET 
app/bins/{postcode}/{house number} 
returns the next collection type for the current date (this or next weeks bin if colleciton date has past)

GET
app/bins/{postcode/{house number}/{bin type}
returns the next date of collection for this bin type

GET
apps/bins/{postcode}/{house number}/{bin type}/period
returns how often the bin is collected, weekly, bi-weekly, every N weeks, etc...

```

Bin Types should be one of the following
- blue
- recycling
- brown
- compost
- black
- glass

Other needless complicated API endpoints may be added over time if only for the sake of building something more complex and
experiment but the above APIs would be the primary ones.

Potential future endpoints

- something to allow creation of a user, by an admin level role, admin should require an API key or bearer token authentication 
handles outside of this app (i.e openid connect or external auth (still using openid connect)
- query similar to above api but without postcode/house number instead relying on bearer token to identify the address details
- allow user to register for webhook callback that can notify the night before one or more bin types are due

