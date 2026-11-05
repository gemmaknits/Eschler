i want to create a SP as SO.P\_SO\_PRICE\_LIST\_PKG\_select\_price\_line

parameters will be

@p\_so\_price\_list\_header\_id bigint,

@p\_design\_no varchar(30),

@p\_color\_code varchar(30),

@p\_uom char(10),

@p\_qty numeric(12,2),

@p\_curr char(10)



Based on these parameters, i want to match the correct row in so.so\_price\_list\_detail.

Matching is done with qty\_min, qty\_max, color tier, currency, uom, design\_no



keep in mind that sometimes the color tier has PFE/PFD, at may also have individual row for PFE, PFD.

If @p\_color\_code='PFE' for example, try to match the individual row. if can't find, get the price from PFE/PFD or PFD/PFE tier.



regarding qty tier, check qty min and qty max.
in case there is ambiguity on qty tier, return entire set of rows for the design\_no for the matching color tier.
if color tier and qty tier are both ambiguous then return all rows for the design\_no. order by so\_price\_list\_header\_id,set\_no, line\_no

return the following columns

so\_price\_list\_header\_id,

so\_price\_list\_detail\_id,

set\_no,

line\_no,

color\_tier,

qty\_min, qty\_max

curr,

price,

notes,

uom



Return rows where activee='Y'



There is one more thing. 

Regarding matching design\_no, first try full design\_no.

if fails, match with partial design

for example design in parameter may 255434AA/11 but so\_price\_list\_detail may have only 255434.



