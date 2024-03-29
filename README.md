# Ventura 
![!Alt_Text](https://i.ibb.co/Ks8Dm46/48260901146-64b8dbb557-o.png)

A .NET Standard library implementing the [Fortuna PRNG](https://en.wikipedia.org/wiki/Fortuna_(PRNG)) as devised by Bruce Schneier and Niels Ferguson in 2003. Also implemented are reseeding improvements suggested by Dodis et al in ["How to Eat Your Entropy and Have it Too"](https://eprint.iacr.org/2014/167).

[![Build Status](https://travis-ci.com/nickpts/Ventura.svg?branch=master)](https://travis-ci.com/nickpts/Ventura)
![Nuget](https://img.shields.io/nuget/v/Ventura.svg)
![GitHub repo size](https://img.shields.io/github/repo-size/nickpts/Ventura.svg)
![GitHub](https://img.shields.io/github/license/nickpts/Ventura.svg)
![GitHub last commit](https://img.shields.io/github/last-commit/nickpts/Ventura.svg)

## Description
How does this differ from existing Fortuna implementations?
  1. Reseeding is pseudo-random rather than cyclical (Dodis et al).
  2. Reading an entropy pool empties multiple pools (Dodis et al).
  3. Support for TwoFish, Serpent and BlowFish in addition to AES (default).
  4. Remote entropy sources ([weather](https://rapidapi.com/community/api/open-weather-map), [radioactive decay](https://www.fourmilab.ch/hotbits/), [atmospheric noise](https://www.random.org/bytes/)) included apart from local.
  
For more info on the above as well as performance, testing and limitations, please see the [wiki](https://github.com/nickpts/Ventura/wiki).
  
## Example
```C#
// seed stream
var stream = new FileStream("seed", FileMode.OpenOrCreate);

// instantiate rng and reads seed
using var prng = RNGVenturaProviderFactory.Create(stream);

// get a random number from 1 to 
int randomNumber = prng.Next(1, 10); 
// new seed will be written to stream at end of using scope
```
## How to run
Prints a random number from 1 to 10, reads/writes seed to seed.bin
```powershell
dotnet Ventura.Cli.dll rn -s seed.bin -i 1 -x 10
```
Prints 100 random numbers from 1 to 10, reads/writes seed to seed.bin
```powershell
dotnet Ventura.Cli.dll rns -s seed.bin -i 1 -x 10 -n 100
```
Docker
```docker
No longer supported due to subscription limitations.
```
## Performance
Time taken to generate 10 random numbers between 1 and 10 using MemoryStream, AES and full entropy reseed.
![!Alt_Text](https://i.ibb.co/GtxnSTZ/49843352616-f06ee80d02-o.png)

## Acknowledgements
[Bouncy Castle](https://www.bouncycastle.org/) for ciphers.
[John Walker](https://www.fourmilab.ch/random/) for ENT testing and HotBits radioactive decay entropy
