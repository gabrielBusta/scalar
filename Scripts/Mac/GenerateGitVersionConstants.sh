VERSIONREGEX="([[:digit:]]+).([[:digit:]]+).([[:digit:]]+).([[:alpha:]]+).([[:digit:]]+).([[:digit:]]+)"
if [[ $1 =~ $VERSIONREGEX ]]
then
        cat >$2/ScalarConstants.GitVersion.cs <<TEMPLATE
// This file is auto-generated by Scalar.PreBuild.GenerateGitVersionConstants. Any changes made directly in this file will be lost.
using Scalar.Common.Git;

namespace Scalar.Common
{
    public static partial class ScalarConstants
    {
        public static readonly GitVersion SupportedGitVersion = new GitVersion(${BASH_REMATCH[1]}, ${BASH_REMATCH[2]}, ${BASH_REMATCH[3]}, "${BASH_REMATCH[4]}", ${BASH_REMATCH[5]}, ${BASH_REMATCH[6]});
    }
}
TEMPLATE
fi
