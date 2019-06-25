# Ventura
A .NET Standard library implementing the [Fortuna PRNG](https://en.wikipedia.org/wiki/Fortuna_(PRNG)) as devised by Bruce Schneier and Niels Ferguson in 2003. Also implemented are reseeding improvements suggested by Dodis et al in ["How to Eat Your Entropy and Have it Too"](https://eprint.iacr.org/2014/167).

[![Build Status](https://travis-ci.com/nickpts/Ventura.svg?branch=master)](https://travis-ci.com/nickpts/Ventura)
[![Known Vulnerabilities](https://snyk.io//test/github/nickpts/Ventura/badge.svg?targetFile=src/Ventura/Ventura.csproj)](https://snyk.io//test/github/nickpts/Ventura?targetFile=src/Ventura/Ventura.csproj)

## Description
How does this differ from existing Fortuna implementations?
  1. Reseeding is pseudo-random rather than cyclical (Dodis et al)
  2. Support for TwoFish, Serpent and BlowFish in addition to AES (default).
  3. Remote entropy sources included apart from local. 

## Entropy sources

## Example
```C#

// by default this instatiates with AES and both local/remote resources.
var prng = PrngVenturaFactory.Create();

// to use
var data = new byte[128];
var random = prng.GetRandomData(data);

// other constructors
var prng = PrngVenturaFactory.Create(
  Cipher.TwoFish, // different ciphers supported
  ReseedEntropySourceGroup.Local, // local, remote or both types of entropy sources
  byte[] seed, // optional user provided seed
  CancellationToken token = default) // stops entropy accumulation
```

## Acknowledgements
[Bouncy Castle](https://www.bouncycastle.org/) is used for the ciphers.
