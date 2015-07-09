# Livefyre C# Utility Classes
[![GitHub version](https://badge.fury.io/gh/livefyre%2Flivefyre-csharp-utils.png)](http://badge.fury.io/gh/livefyre%2Flivefyre-csharp-utils)
[![Circle CI](https://circleci.com/gh/Livefyre/livefyre-csharp.png?style=badge)](https://circleci.com/gh/Livefyre/livefyre-csharp-utils)
[![Coverage Status](https://img.shields.io/coveralls/Livefyre/livefyre-csharp-utils.svg)](https://coveralls.io/r/Livefyre/livefyre-csharp-utils)

Livefyre's official library for common server-side tasks necessary for getting Livefyre apps (comments, reviews, etc.) working on your website.

Works with C# Framework X and later. X likely 4.0 (possibly 3.5).

## Installation

TODO: Instructions.

TODO: Dependencies.

## Documentation

TODO: Located [here](http://answers.livefyre.com/developers/libraries).

## Contributing

1. Fork it
2. Create your feature branch (`git checkout -b my-new-feature`)
3. Commit your changes (`git commit -am 'Add some feature'`)
4. Push to the branch (`git push origin my-new-feature`)
5. Create new Pull Request

Note: any feature update on any of Livefyre's libraries will need to be reflected on all libraries. We will try and accommodate when we find a request useful, but please be aware of the time it may take.

## License

MIT



### NOTES/THOUGHTS (WIP):

A translation from our Java library that hopes to become idiomatic C#, with help:
https://github.com/Livefyre/livefyre-java-utils


As a library, I would love to be better, continually improve and evolve. I am very grateful for any help you may spare on my journey.


TODO: DRY ME!

Should we:
	Use Code Contracts?
		requires a runtime checking Setting
	Reduce Deps?
	Pull Init/Create behavior into Constructors?
	Refactor the WebRequests into a general method?


Network.cs: 
	Ln106 - make configurable/pull out mutable api v3_0 key?



 