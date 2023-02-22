Change Log / Release Notes
==========================

Latest Version
==============

1.0.0 (2023-02-22)
------------------
* Added synchronous Compensator.
* Added documentation comments to all public methods and properties.
* Removed obsolete asynchronous Compensator.  Functionality was previously replicated to AsyncCompensator.

Previous Versions
=================

0.5.1-beta-5 (2022-11-11)
-------------------------
* Updated ForeachAsync to define its generic parameter as TItem instead of T.

0.5.0-beta-5 (2022-11-07)
-------------------------
* Internal refactor.

0.4.0-beta-4 (2022-11-05)
-------------------------
* Replicated asynchronous Compensator functionality to AsyncCompensator,
* Deprecated asynchronous Compensator.

0.3.1-beta-3 (2022-10-10)
------------------------
* Updated practical example in README.md.

0.3.0-beta-3 (2022-10-10)
------------------------
* Internal refactor.

0.2.4-beta-2 (2022-10-07)
-------------------------
* Updated README.md.

0.2.3-beta-2 (2022-10-07)
-------------------------
* Added README.md to nuget package.

0.2.2-beta-1 (2022-10-04)
-------------------------
* Internal refactor.

0.2.1-beta-1 (2022-10-04)
-------------------------
* Internal refactor.

0.2.0-beta-1 (2022-10-04)
-------------------------
* Added ForEachAsync feature.
* Added CommitAsync feature.

0.1.1-beta-1 (2022-09-25)
-------------------------
* Changed Tag.Label and CreateTagAsync(string label) overload to internal as the label has little value except for debugging the compensation stack.

0.1.0-beta-1 (2022-09-22)
-------------------------
* Added CreateTagAsync feature.
* Added CompensateAsync feature.
* Added missing overloads.
* Changed Status to an enum instead of a string.

0.0.0-alpha-1 (2022-09-20)
--------------------------
* Added asynchronous Compensator with AddCompensationAsync, DoAsync, DoIfAsync, and GetAsync features.




