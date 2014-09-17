reflection-descriptors
======================

.NET Framework for dynamically inspecting assemblies without loading them into the current AppDomain which enables unloading of the assembly again.

This is an ongoing port of an old lirbrary I wrote to manage loading and inspection on Assemblies for plugin systems, where one wishes to host the plugins in eihter separated AppDomains or Processes.
