# SHJI

This is a C# implementation of the Jane Language.

SHJI stands for Schleswig-Holstein Jane Interpreter, which is a pendant to the name of the Haskell Compiler which is also named after a geographical location.

It will feature:

* A REPL
* File interpreter (TODO)
* General CLI (TODO)

## Jane

The Design Goals of Jane can be viewed [here](https://github.com/nora2605/jane).

This interpreter is NOT complete and does not represent the design of the Jane Language in its entirety. That stage will come eventually.

## Details

It's a simple Tree-Walking interpreter. There is no pre-optimization. This might change at a later stage.

## TODO

The following constitutes a list of features that still have to be done:

### General

* Operators:
	* Switcheroo operator: switches a bool from true to false or false to true (postfix !!)
	* Modulo
	* Binary arithmetic
	* Boolean arithmetic
	* Pufferfish
	* Type coercion
	* Range Operator
* Type Parsing:
	* Number suffixes

### Parser

* Parse Lambda expressions
* Parse Accessor Expressions
* Parse Object Literals and Tuples
* Parse Control flow (for, loop, while)
* Parse Preprocessor Directives
* Parse #/Maths functions

### Interpreter

Transition from Tree-Walking to Bytecode (speed)

* A proper type-system (I have to think about a way I can evaluate operators without hardcoding every number type)						
    * To that: Maybe have a file called Builtins\/<NumberType\>.cs that contains a lot of Operators so it doesn't clog up the Main Interpreter file
	* Yeah actually Builtins/Type.cs seems like a good way to have a standard library of sorts.
	* Also Rewrite the ObjectType enum to be a class to support generics, inbuilts, all that
		* That also means there will be no "Jane..." structs anymore, just JaneObject with a specific type that is linked to the standard library
		* Maybe the primitives have to stay, i don't know
* External Functions (file use directives)
* Object Orientation (basically)
* Type enforcement for Arrays
* Type inference for objects
* Custom Operator Overloading
* Extension Blocks
* MOST COMPLICATED TYPE SYSTEM INFER ALGORITHM: (Extendable by Runtime)
	* Pseudocode
```
	InferTypes(BinaryOperator, Type1, Type2?, depth=0) {
		OperatorDict = OperatorImplementations[BinaryOperator]
		if (TryGet OperatorDict[(Type1, Type2?)])
			return (OperatorDict[(Type1, Type2?)].Function, (OperatorDict[(Type1, Type2,...)].OutType, Type1, Type2, ...))
		foreach Type get ImplicitTypeConversions
			foreach CombinationOf ImplicitTypeConversions
				InferTypes(depth = depth + 1) foreach Combination
		return Combination.Sort(by depth).Fst()
	}
```

## Credit

HUGE Thanks to the [Writing an Interpreter in Go](https://interpreterbook.com/) Book by Thorsten Ball. Most of the code (as of July 2023) is a slightly altered workthrough of that Book.

I encourage everyone who also wants to build their own language to read this book, it gives enough insight that your own changes to the behavior of the Language seem intuitive.

