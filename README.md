# Ventura
A .NET Standard library implementing the [Fortuna PRNG](https://en.wikipedia.org/wiki/Fortuna_(PRNG)) as devised by Bruce Schneier and Niels Ferguson in 2003. Also implemented are reseeding improvements suggested by Dodis et al in ["How to Eat Your Entropy and Have it Too"](https://eprint.iacr.org/2014/167).

[![Build Status](https://travis-ci.com/nickpts/Ventura.svg?branch=master)](https://travis-ci.com/nickpts/Ventura)
![Nuget](https://img.shields.io/nuget/v/Ventura.svg)
![GitHub repo size](https://img.shields.io/github/repo-size/nickpts/Ventura.svg)
![GitHub](https://img.shields.io/github/license/nickpts/Ventura.svg)
![GitHub last commit](https://img.shields.io/github/last-commit/nickpts/Ventura.svg)
[![Known Vulnerabilities](https://snyk.io//test/github/nickpts/Ventura/badge.svg?targetFile=src/Ventura/Ventura.csproj)](https://snyk.io//test/github/nickpts/Ventura?targetFile=src/Ventura/Ventura.csproj)

## Description
How does this differ from existing Fortuna implementations?
  1. Reseeding is pseudo-random rather than cyclical (Dodis et al)
  2. Support for TwoFish, Serpent and BlowFish in addition to AES (default).
  3. Remote entropy sources included apart from local. 

## Entropy sources

## Example
```C#

// by default this instatiates with AES and local entropy sources and writes the seed to memory
using (var prng = PrngVenturaFactory.Create(new MemoryStream()))
{
    // to use
    var data = new byte[128];
    byte[] random = prng.GetRandomData(data); 
    int randomNumber = prng.GetRandomNumber();
    int[] randomNumbers = prng.GetRandomNumbers
}

// optional parameters
var prng = PrngVenturaFactory.Create(
  Stream stream, // stream to read/write seed from/to
  Cipher.TwoFish, // different block ciphers supported
  ReseedEntropySourceGroup.Full); // local, remote or both types of entropy sources
```

## Acknowledgements
[Bouncy Castle](https://www.bouncycastle.org/) is used for the ciphers.
