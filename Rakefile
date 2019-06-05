# --------------------------------------------------------------------------- #
#
# Copyright (c) 2010 CubeSoft, Inc.
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#  http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
#
# --------------------------------------------------------------------------- #
require 'rake'
require 'rake/clean'

# --------------------------------------------------------------------------- #
# configuration
# --------------------------------------------------------------------------- #
PROJECT     = "Cube.Pdf"
MAIN        = "#{PROJECT}.Apps"
LIB         = "../packages"
NATIVE      = "../resources/native"
BRANCHES    = ["master", "net35"]
FRAMEWORKS  = ["net45", "net35"]
CONFIGS     = ["Release", "Debug"]
PLATFORMS   = ["Any CPU", "x86", "x64"]
GS_NAME     = "gsdll32.dll"
GS_DEST     = ["Libraries/Tests", "Applications/Converter/Tests", "Applications/Converter/Main"]
PDFIUM_NAME = "pdfiumviewer.native"
PDFIUM_KIND = "no_v8-no_xfa"
PDFIUM_VER  = "2018.4.8.256"
PDFIUM_DEST = ["Libraries/Tests", "Applications/Editor/Tests", "Applications/Editor/Main"]
PACKAGES    = ["Libraries/Core/#{PROJECT}.Core.nuspec",
               "Libraries/Ghostscript/#{PROJECT}.Ghostscript.nuspec",
               "Libraries/Itext/#{PROJECT}.Itext.nuspec",
               "Libraries/Pdfium/#{PROJECT}.Pdfium.nuspec"]
TESTCASES   = {"#{PROJECT}.Tests"            => "Libraries/Tests",
               "#{PROJECT}.Converter.Tests"  => "Applications/Converter/Tests",
               "#{PROJECT}.Editor.Tests"     => "Applications/Editor/Tests",
               "#{PROJECT}.Pinstaller.Tests" => "Applications/Pinstaller/Tests"}

# --------------------------------------------------------------------------- #
# commands
# --------------------------------------------------------------------------- #
BUILD = "msbuild -v:m -t:build -p:Configuration=#{CONFIGS[0]}"
PACK  = %(nuget pack -Properties "Configuration=#{CONFIGS[0]};Platform=AnyCPU")
TEST  = "../packages/NUnit.ConsoleRunner/3.10.0/tools/nunit3-console.exe"

# --------------------------------------------------------------------------- #
# clean
# --------------------------------------------------------------------------- #
CLEAN.include("#{PROJECT}.*.nupkg")
CLEAN.include("#{LIB}/cube.*")
CLEAN.include(["bin", "obj"].map{ |e| "**/#{e}" })

# --------------------------------------------------------------------------- #
# default
# --------------------------------------------------------------------------- #
desc "Clean, build, and create NuGet packages."
task :default => [:clean, :build_all, :pack]

# --------------------------------------------------------------------------- #
# pack
# --------------------------------------------------------------------------- #
desc "Create NuGet packages in the net35 branch."
task :pack do
    checkout("net35") { PACKAGES.each { |e| sh("#{PACK} #{e}") }}
end

# --------------------------------------------------------------------------- #
# build
# --------------------------------------------------------------------------- #
desc "Build projects in the current branch."
task :build, [:platform] do |_, e|
    e.with_defaults(:platform => PLATFORMS[0])
    sh("nuget restore #{MAIN}.sln")
    sh(%(#{BUILD} -p:Platform="#{e.platform}" #{MAIN}.sln))
end

# --------------------------------------------------------------------------- #
# clean_build
# --------------------------------------------------------------------------- #
desc "Build projects in pre-defined branches and platforms."
task :build_all do
    BRANCHES.product(PLATFORMS) { |set|
        checkout(set[0]) do
            Rake::Task[:build].reenable
            Rake::Task[:build].invoke(set[1])
        end
    }
end

# --------------------------------------------------------------------------- #
# build_test
# --------------------------------------------------------------------------- #
desc "Build and test projects in the current branch."
task :build_test => [:build, :test]

# --------------------------------------------------------------------------- #
# test
# --------------------------------------------------------------------------- #
desc "Build and test projects in the current branch."
task :test do
    fw  = %x(git symbolic-ref --short HEAD).chomp
    fw  = 'net45' if (fw != 'net35')
    bin = ['bin', PLATFORMS[0], CONFIGS[0], fw].join('/')
    TESTCASES.each { |p, d| sh(%(#{TEST} "#{d}/#{bin}/#{p}.dll" --work="#{d}/#{bin}")) }
end

# --------------------------------------------------------------------------- #
# test_all
# --------------------------------------------------------------------------- #
desc "Test projects in pre-defined branches."
task :test_all do
    BRANCHES.each { |e| checkout(e) { Rake::Task[:test].execute }}
end

# --------------------------------------------------------------------------- #
# Copy
# --------------------------------------------------------------------------- #
desc "Copy resources to the bin directories."
task :copy, [:platform, :framework] do |_, e|
    v0 = (e.platform  != nil) ? [e.platform ] : PLATFORMS
    v1 = (e.framework != nil) ? [e.framework] : FRAMEWORKS

    v0.product(CONFIGS, v1) { |set|
        pf  = (set[0] == 'Any CPU') ? 'x64' : set[0]
        bin = ['bin', set[0], set[1], set[2]].join('/')

        GS_DEST.each { |root|
            src  = "#{NATIVE}/#{pf}/gs/#{GS_NAME}"
            dest = "#{root}/#{bin}"
            RakeFileUtils::mkdir_p(dest)
            RakeFileUtils::cp_r(src, dest)
        }

        PDFIUM_DEST.each { |root|
            name = [PDFIUM_NAME, (pf == 'x64') ? 'x86_64' : 'x86', PDFIUM_KIND].join('.')
            src  = [LIB, name, PDFIUM_VER, 'Build', pf, 'pdfium.dll'].join('/')
            dest = "#{root}/#{bin}"
            RakeFileUtils::mkdir_p(dest)
            RakeFileUtils::cp_r(src, dest)
        }
    }
end

# --------------------------------------------------------------------------- #
# checkout
# --------------------------------------------------------------------------- #
def checkout(branch, &callback)
    sh("git checkout #{branch}")
    callback.call()
ensure
    sh("git checkout master")
end
