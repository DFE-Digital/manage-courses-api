// usage: 
// node index https://bat-dev-search-and-compare-api-app.azurewebsites.net abc_the_api_key

var fs = require('fs');
var request = require('request');

var data = JSON.parse(fs.readFileSync('../out.json', {encoding: "utf8"}));

data = data.filter(x => x.CourseSubjects && x.CourseSubjects.length);

console.log(data.length);

var options = {
	method: "POST",
	json: data,
	url: process.argv[2] + "/api/courses",
	headers: {		
		"Authorization": "Bearer " + process.argv[3],
		"Content-Type": "application/json"		
	}
};

request(options, (err, res, body) => {
	console.log(err);
	console.log(res.statusCode);	
	console.log(body);
}) 