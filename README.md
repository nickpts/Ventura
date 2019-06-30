# Ventura
A .NET Standard library implementing the [Fortuna PRNG](https://en.wikipedia.org/wiki/Fortuna_(PRNG)) as devised by Bruce Schneier and Niels Ferguson in 2003. Also implemented are reseeding improvements suggested by Dodis et al in ["How to Eat Your Entropy and Have it Too"](https://eprint.iacr.org/2014/167).

[![Build Status](https://travis-ci.com/nickpts/Ventura.svg?branch=master)](https://travis-ci.com/nickpts/Ventura)
![Nuget](https://img.shields.io/nuget/v/Ventura.svg)
![GitHub repo size](https://img.shields.io/github/repo-size/nickpts/Ventura.svg)
![GitHub](https://img.shields.io/github/license/nickpts/Ventura.svg)
![GitHub last commit](https://img.shields.io/github/last-commit/nickpts/Ventura.svg)

## Description
How does this differ from existing Fortuna implementations?
  1. Reseeding is pseudo-random rather than cyclical (Dodis et al)
  2. Support for TwoFish, Serpent and BlowFish in addition to AES (default).
  3. Remote entropy sources included apart from local. 

## Entropy sources

## Example
```C#

// instantiate and write the seed to memory
using (var prng = RNGVenturaServiceProviderFactory.Create(new MemoryStream()))
{
    var data = new byte[128];
    prng.GetBytes(data); 
    
    // get a random number from 0 to 10
    int randomNumber = prng.GetRandomNumber(0, 10); 
    
    // get an array with 100 random numbers from 0 to 10
    int[] randomNumbers = prng.GetRandomNumbers(0, 10, 100); 
}

// optional parameters
var prng = RNGVenturaServiceProviderFactory.Create(
  Stream stream, // stream to read/write seed from/to
  Cipher.TwoFish, // different block ciphers supported
  ReseedEntropySourceGroup.Full); // local, remote or both types of entropy sources
```
## Performance
Test conditions: local entropy sources, AES cipher, FileStream used for seed operations.
![Alt text](https://live.staticflickr.com/65535/48153972572_2d5f6bbc26_o_d.jpg)

## Acknowledgements
[Bouncy Castle](https://www.bouncycastle.org/) is used for the ciphers.
