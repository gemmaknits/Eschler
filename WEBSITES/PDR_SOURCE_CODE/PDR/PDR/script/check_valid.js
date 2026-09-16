<!--
//check the valid integer
function valid_integer(zz) 
{
  var digits="0123456789."
  
  var temp;

  for (var i=0;i<zz.length;i++) {
        temp=zz.substring(i,i+1)
        if (digits.indexOf(temp)==-1) 
		{
                return true;
        }
  }
return false;
}

function valid_integer_dot(zz) 
{
  var digits="0123456789.,"
  var temp;

  for (var i=0;i<zz.length;i++) {
        temp=zz.substring(i,i+1)
        if (digits.indexOf(temp)==-1) 
		{
                return true;
        }
  }
return false;
}

function isNumber(z)
{
    return isNaN(z);
}
//-->