//
// Copyright © 2021 Terry Moreland
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSMoreland.ArdsBorough.Api.WebServiceFacade.Abstractions.Contracts;

/// <summary>
/// Factory producing <see cref="IWebServiceFacade"/> instances.
/// </summary>
public interface IWebServiceFacadeFactory
{
    /// <summary>
    /// Constructs an instance of <see cref="IWebServiceFacade"/> using
    /// <paramref name="username"/> and <paramref name="password"/>
    /// for authorization
    /// </summary>
    /// <param name="username">username used to authorize request</param>
    /// <param name="password">password used to authorize requed</param>
    /// <returns>an instance of <see cref="IWebServiceFacade"/></returns>
    /// <exception cref="ArgumentException">
    /// <list type="bullet">
    ///   <item>
    ///     <description>
    ///     if <paramref name="username"/> is <see langword="null"/> or <see cref="string.Empty"/>
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     if <paramref name="password"/> is <see langword="null"/> or <see cref="string.Empty"/>
    ///     </description>
    ///   </item>
    /// </list>
    /// </exception>
    IWebServiceFacade Build(string username, string password);
}
