# UmbCanonicalUrlRedirect
Plugin/event handler for Umbraco that forces pages to redirect to canonical URLs and adds trailing slashes if needed.

## What's inside
This project includes a DLL that will register as an event handler with Umbraco. This will automatically redirect users to canonical URLs for the pages they request. It will also rewrite trailing slashes to match the configured value in **umbracoSettings.config**.

## System requirements
1. NET Framework 4.5
2. Umbraco 7.3.7+ (should work with older versions but not tested)

## NuGet availability
This project is available on [NuGet](https://www.nuget.org/packages/UmbCanonicalUrlRedirect/).

## Usage instructions
### Getting started
1. Add **UmbCanonicalUrlRedirect.dll** as a reference in your project or place it in the **\bin** folder.
2. If you wish to be able prevent specific pages from being redirected, add a true/false-type document property in Umbraco:
  - `umbNoCanonicalRedirect`
