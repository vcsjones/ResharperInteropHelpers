## What Is It?
Resharper Interop Assist is a [Resharper](http://www.jetbrains.com/resharper) Plug In that assists in writing and consuming Interop code. Interop is tricky, and easy to get wrong. This tool aims to catch these problems, resulting in fewer bugs and less hairpulling.

## What Does It Do?
It provides the developer warnings and quickfixes for common interop problems that don't immediately present themselves as a bug. Take this imported method declaration:

```
[DllImport("kernel32.dll", EntryPoint = "CloseHandle")]
public static extern void CloseHandle(int hObject);
```

[CloseHandle](http://msdn.microsoft.com/en-us/library/windows/desktop/ms724211.aspx) takes a platform-dependent sized integer, however in the delcaration we declare it with a 32-bit integer. This declaration will work fine if run as x86, however when run against the x64 CLR, the incorrect marshalling will result in CloseHandle trying to close an incorrect handle. Interop Assist will warn the developer of this scenario:

![Resharper Interop Assist](http://vcsjones.files.wordpress.com/2012/05/image.png "Screenshot")

Resharper Interop Assist keeps a dictionary of how the parameters should be marshalled for well-known libraries, such as kernel32.dll.

Other functionality includes warnings when the [DllImport] attribute is missing, or is not declared static.

## What Is The Goal?
The goal of Resharper Interop Assist is to provide as much assistance as possible with incorrect marshalling.

## How Do I Install It?
Not at the moment. Right now the project is currently too incomplete to focus on this. However, if you want to try it out, you can download the [Resharper SDK](http://www.jetbrains.com/resharper/download/) and build it yourself.

##License

![Creative Commons License](http://i.creativecommons.org/l/by-nc-sa/3.0/88x31.png "Creative Commons License")

Resharper Interop Assist by [Kevin Jones](http://vcsjones.com) is licensed under a [Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License](http://creativecommons.org/licenses/by-nc-sa/3.0/).