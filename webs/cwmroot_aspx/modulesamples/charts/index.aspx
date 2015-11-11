<html>
<body style="font-family: Arial">

<h1>Chart demos</h1>
<p><A href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/charts/index.src">View source code of the page</A></p>
<h2>Sample 1</h2>
<p>Small bar, no legend<br><img src="/system/modules/charts/chartgenerator.aspx?xValues=Mo|Di|Mittwoch|Do|Fr|Sa|So&yValues=10|027.00|20.00|95.00|10.00|20.50|3&charttype=bar&width=150&height=120" width="150" height="120"
border="0" alt="Sample 1"></p>

<h2>Sample 2</h2>
<p>Small pie<br><img src="/system/modules/charts/chartgenerator.aspx?xValues=Beverages|Condi.|Confe.|DairyPr.|Grains|Poultry|Produce|Seafood&yValues=267|180|107|848|167|221|207|285&ChartType=pie&legend=false&width=300&height=170&yunit=USD&drawtotal=false" width="300" height="170"
border="0" alt="Sample 2"></p>

<h2>Sample 3</h2>
<p>Default stylish of bar graphs, only data values given<br><img src="/system/modules/charts/chartgenerator.aspx?xValues=Beverages|Condiments|Confections|DairyProducts|Grains/Cereals|Meat/Poultry|Produce|Seafood&yValues=267868|180919|106047|849548|167357|221581|234507|217285&ChartType=bar"
border="0" alt="Sample 3"></p>

<h2>Sample 4</h2>
<p>Large bars with legend, unit for y-values<br><img src="/system/modules/charts/chartgenerator.aspx?xValues=Beverages|Condiments|Confections|DairyProducts|Grains/Cereals|Meat/Poultry|Produce|Seafood&yValues=267868|180919|106047|849548|167357|221581|234507|217285&ChartType=bar&legend=true&width=700&height=400&yunit=USD" width="700" height="400"
border="0" alt="Sample 4"></p>

<h2>Sample 5</h2>
<p>Large pie chart, unit for y-values<br><img src="/system/modules/charts/chartgenerator.aspx?xValues=Beverages|Condiments|Confections|DairyProducts|Grains/Cereals|Meat/Poultry|Produce|Seafood&yValues=267868|180919|106047|849548|167357|221581|234507|217285&ChartType=pie&width=625&height=250&yunit=EUR&drawtotal=true" width="625" height="250"
border="0" alt="Sample 5"></p>

<h2>Sample 6</h2>
<p>Large pie chart, no unit for y-values but formatted numbers, no calculation of the totals<br><img src="/system/modules/charts/chartgenerator.aspx?xValues=Beverages|Condiments|Confections&yValues=581|237|285&ChartType=pie&bgcolor=FFFDFE&width=700&height=400&numberformat=<%= Server.URLEncode("0 TEUR") %>&drawtotal=false" width="700" height="400"
border="0" alt="Sample 6"></p>

<h2>Sample 7</h2>
<p>Large pie chart, no unit for y-values but formatted numbers, no calculation of the totals, 12 months<br><img src="/system/modules/charts/chartgenerator.aspx?xValues=January|February|March|April|May|June|July|August|September|October|November|December&yValues=0.11548|0.091239|.05417|0.07068|0.0847|0.12151|0.0754|0.08145|0.06719|0.0477|0.0948|0.09567&ChartType=pie&bgcolor=FFFDFE&width=700&height=400&numberformat=<%= Server.URLEncode("0.00 %") %>&drawtotal=false" width="700" height="400"
border="0" alt="Sample 7"></p>

</body>
</html>