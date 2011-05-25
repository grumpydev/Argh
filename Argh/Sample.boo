Iterations = 1

RequestConfiguration "Basic Strings":
	method @Get
	url "http://localhost:40965/test"
	no_cache true
	headers { 
		Testing:"my value",
		AnotherHeader:"another value"
	}

RequestConfiguration "Spark View":
	method @Post
	url "http://localhost:40965/spark"
	no_cache true
	content_type "application/x-www-form-urlencoded"
	body "testing=qwdqw"
