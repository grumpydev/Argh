# Introduction

ARGH is a simple stress test / benchmarking tool for http web applications/services. It uses a Boo DSL to define the tests to run, and outputs error counts, concurrent connection counts and requests per second.

As example of the DSL is:

```boo
﻿Iterations = 1

RequestConfiguration "Get Request":
	method @Get
	url "http://localhost:40965/test"
	no_cache true
	headers { 
		Testing:"my value",
		AnotherHeader:"another value"
	}

RequestConfiguration "Manual Form Post":
	method @Post
	url "http://localhost:40965/spark"
	no_cache true
	content_type "application/x-www-form-urlencoded"
	body "testing=qwdqw"

RequestConfiguration "Automatic Form Post":
	url "http://localhost:40965/spark"
	no_cache true
	post_form {
		testing:"field value",
		anotherTest:"another field value"
	}
```