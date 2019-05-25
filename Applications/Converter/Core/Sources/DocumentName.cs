﻿/* ------------------------------------------------------------------------- */
//
// Copyright (c) 2010 CubeSoft, Inc.
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
/* ------------------------------------------------------------------------- */
using Cube.FileSystem;
using Cube.Mixin.String;
using System.Diagnostics;

namespace Cube.Pdf.Converter
{
    /* --------------------------------------------------------------------- */
    ///
    /// DocumentName
    ///
    /// <summary>
    /// Provides functionality to use the provided document name as a
    /// filename.
    /// </summary>
    ///
    /// <remarks>
    /// ドキュメント名をファイル名の初期値として利用する時に利用不可能な
    /// 文字の除去等の処理を担います。
    /// </remarks>
    ///
    /* --------------------------------------------------------------------- */
    public sealed class DocumentName
    {
        #region Constructors

        /* ----------------------------------------------------------------- */
        ///
        /// DocumentName
        ///
        /// <summary>
        /// Initializes a new instance of the DocumentName class with the
        /// specified arguments.
        /// </summary>
        ///
        /// <param name="src">Original document name.</param>
        /// <param name="alternate">Default filename.</param>
        /// <param name="io">I/O handler.</param>
        ///
        /* ----------------------------------------------------------------- */
        public DocumentName(string src, string alternate, IO io)
        {
            IO           = io;
            DefaultValue = alternate;
            _filter      = new PathFilter(src)
            {
                AllowCurrentDirectory = false,
                AllowDriveLetter      = false,
                AllowInactivation     = false,
                AllowParentDirectory  = false,
                AllowUnc              = false,
            };
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Source
        ///
        /// <summary>
        /// Gets the original document name.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string Source => _filter.Source;

        /* ----------------------------------------------------------------- */
        ///
        /// Value
        ///
        /// <summary>
        /// Gets a name that can be used as a filename.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string Value => GetValue();

        /* ----------------------------------------------------------------- */
        ///
        /// DefaultValue
        ///
        /// <summary>
        /// Gets a name that is used when the specified document name
        /// cannot be used as a filename.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string DefaultValue { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// IO
        ///
        /// <summary>
        /// Gets the I/O handler.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IO IO { get; }

        #endregion

        #region Methods

        /* ----------------------------------------------------------------- */
        ///
        /// GetValue
        ///
        /// <summary>
        /// Gets a name that is used as a filename.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private string GetValue()
        {
            if (!Source.HasValue()) return DefaultValue;

            var dest = IO.Get(_filter.Result).Name;
            var key  = " - ";
            var pos  = dest.LastIndexOf(key);
            if (pos == -1) return dest;

            var head = dest.Substring(0, pos);
            var tail = dest.Substring(pos);

            if (System.IO.Path.HasExtension(head)) return head;
            if (System.IO.Path.HasExtension(tail))
            {
                Debug.Assert(tail.StartsWith(key));
                return tail.Substring(key.Length);
            }
            return dest;
        }

        #endregion

        #region Fields
        private readonly PathFilter _filter;
        #endregion
    }
}
