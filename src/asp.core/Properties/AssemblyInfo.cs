﻿using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("asp.core.Views")]    // for CalculatorContext.Map1 in the Razor engine
[assembly: InternalsVisibleTo("test.core")]
[assembly: InternalsVisibleTo("test.core.apps")]
[assembly: InternalsVisibleTo("asptest.core")]