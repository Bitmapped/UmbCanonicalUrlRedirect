# UmbCanonicalUrlRedirect
Plugin/event handler for Umbraco that forces pages to redirect to canonical URLs and adds trailing slashes if needed.

## System requirements
1. NET Framework 4.5
2. Umbraco 7.3.x or newer (may work with older versions)

## NuGet availability
This project is available on [NuGet](https://www.nuget.org/packages/UmbCanonicalUrlRedirect/).

## Usage instructions
### Getting started
1. Add **UmbCanonicalUrlRedirect.dll** as a reference in your project or place it in the **\bin** folder.
2. If you wish to be able prevent specific pages from being redirected, add a true/false-type document property in Umbraco:
  - `umbNoCanonicalRedirect`
