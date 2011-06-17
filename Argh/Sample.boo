Iterations = 1

RequestConfiguration "Google":
	method @Get
	url "http://www.google.com/"
	no_cache true

RequestConfiguration "Blog":
	method @Get
	url "http://www.grumpydev.com/"
	no_cache true
