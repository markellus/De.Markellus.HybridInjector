/*  Copyright (C) 2008-2018 Peter Palotas, Jeffrey Jangli, Alexandr Normuradov
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy 
 *  of this software and associated documentation files (the "Software"), to deal 
 *  in the Software without restriction, including without limitation the rights 
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 *  copies of the Software, and to permit persons to whom the Software is 
 *  furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in 
 *  all copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 *  THE SOFTWARE. 
 */

using System;
using System.Security;

namespace Alphaleonis.Win32.Filesystem
{
   public static partial class Directory
   {
      #region .NET

      /// <summary>Gets the date and time that the specified directory was last accessed.</summary>
      /// <returns>A <see cref="System.DateTime"/> structure set to the date and time that the specified directory was last accessed. This value is expressed in local time.</returns>
      /// <param name="path">The directory for which to obtain access date and time information.</param>
      [SecurityCritical]
      public static DateTime GetLastAccessTime(string path)
      {
         return File.GetLastAccessTimeCore(null, path, false, PathFormat.RelativePath).ToLocalTime();
      }

      /// <summary>Gets the date and time, in coordinated universal time (UTC), that the specified directory was last accessed.</summary>
      /// <returns>A <see cref="System.DateTime"/> structure set to the date and time that the specified directory was last accessed. This value is expressed in UTC time.</returns>
      /// <param name="path">The directory for which to obtain access date and time information.</param>
      [SecurityCritical]
      public static DateTime GetLastAccessTimeUtc(string path)
      {
         return File.GetLastAccessTimeCore(null, path, true, PathFormat.RelativePath);
      }

      #endregion // .NET

      /// <summary>[AlphaFS] Gets the date and time that the specified directory was last accessed.</summary>
      /// <returns>A <see cref="System.DateTime"/> structure set to the date and time that the specified directory was last accessed. This value is expressed in local time.</returns>
      /// <param name="path">The directory for which to obtain access date and time information.</param>
      /// <param name="pathFormat">Indicates the format of the path parameter(s).</param>
      [SecurityCritical]
      public static DateTime GetLastAccessTime(string path, PathFormat pathFormat)
      {
         return File.GetLastAccessTimeCore(null, path, false, pathFormat).ToLocalTime();
      }



      /// <summary>[AlphaFS] Gets the date and time, in coordinated universal time (UTC), that the specified directory was last accessed.</summary>
      /// <returns>A <see cref="System.DateTime"/> structure set to the date and time that the specified directory was last accessed. This value is expressed in UTC time.</returns>
      /// <param name="path">The directory for which to obtain access date and time information.</param>
      /// <param name="pathFormat">Indicates the format of the path parameter(s).</param>
      [SecurityCritical]
      public static DateTime GetLastAccessTimeUtc(string path, PathFormat pathFormat)
      {
         return File.GetLastAccessTimeCore(null, path, true, pathFormat);
      }

      #region Transactional

      /// <summary>[AlphaFS] Gets the date and time that the specified directory was last accessed.</summary>
      /// <returns>A <see cref="System.DateTime"/> structure set to the date and time that the specified directory was last accessed. This value is expressed in local time.</returns>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory for which to obtain access date and time information.</param>
      [SecurityCritical]
      public static DateTime GetLastAccessTimeTransacted(KernelTransaction transaction, string path)
      {
         return File.GetLastAccessTimeCore(transaction, path, false, PathFormat.RelativePath).ToLocalTime();
      }

      /// <summary>[AlphaFS] Gets the date and time that the specified directory was last accessed.</summary>
      /// <returns>A <see cref="System.DateTime"/> structure set to the date and time that the specified directory was last accessed. This value is expressed in local time.</returns>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory for which to obtain access date and time information.</param>
      /// <param name="pathFormat">Indicates the format of the path parameter(s).</param>
      [SecurityCritical]
      public static DateTime GetLastAccessTimeTransacted(KernelTransaction transaction, string path, PathFormat pathFormat)
      {
         return File.GetLastAccessTimeCore(transaction, path, false, pathFormat).ToLocalTime();
      }



      /// <summary>[AlphaFS] Gets the date and time, in coordinated universal time (UTC), that the specified directory was last accessed.</summary>
      /// <returns>A <see cref="System.DateTime"/> structure set to the date and time that the specified directory was last accessed. This value is expressed in UTC time.</returns>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory for which to obtain access date and time information.</param>
      [SecurityCritical]
      public static DateTime GetLastAccessTimeUtcTransacted(KernelTransaction transaction, string path)
      {
         return File.GetLastAccessTimeCore(transaction, path, true, PathFormat.RelativePath);
      }

      /// <summary>[AlphaFS] Gets the date and time, in coordinated universal time (UTC), that the specified directory was last accessed.</summary>
      /// <returns>A <see cref="System.DateTime"/> structure set to the date and time that the specified directory was last accessed. This value is expressed in UTC time.</returns>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory for which to obtain access date and time information.</param>
      /// <param name="pathFormat">Indicates the format of the path parameter(s).</param>
      [SecurityCritical]
      public static DateTime GetLastAccessTimeUtcTransacted(KernelTransaction transaction, string path, PathFormat pathFormat)
      {
         return File.GetLastAccessTimeCore(transaction, path, true, pathFormat);
      }
      
      #endregion // Transactional
   }
}
