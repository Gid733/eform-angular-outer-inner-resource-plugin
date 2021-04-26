#!/bin/bash
cd ~
pwd

rm -fR Documents/workspace/microting/eform-angular-frontend/eform-client/src/app/plugins/modules/outer-inner-resource-pn

cp -a Documents/workspace/microting/eform-angular-outer-inner-resource-plugin/eform-client/src/app/plugins/modules/outer-inner-resource-pn Documents/workspace/microting/eform-angular-frontend/eform-client/src/app/plugins/modules/outer-inner-resource-pn

rm -fR Documents/workspace/microting/eform-angular-frontend/eFormAPI/Plugins/OuterInnerResource.Pn

cp -a Documents/workspace/microting/eform-angular-outer-inner-resource-plugin/eFormAPI/Plugins/OuterInnerResource.Pn Documents/workspace/microting/eform-angular-frontend/eFormAPI/Plugins/OuterInnerResource.Pn

# Test files rm
rm -fR Documents/workspace/microting/eform-angular-frontend/eform-client/e2e/Tests/outer-inner-resource-settings
rm -fR Documents/workspace/microting/eform-angular-frontend/eform-client/e2e/Tests/outer-inner-resource-general
rm -fR Documents/workspace/microting/eform-angular-frontend/eform-client/e2e/Page\ objects/OuterInnerResource
rm -fR Documents/workspace/microting/eform-angular-frontend/eform-client/wdio-plugin-step2.conf.js

# Test files cp
cp -a Documents/workspace/microting/eform-angular-outer-inner-resource-plugin/eform-client/e2e/Tests/outer-inner-resource-settings Documents/workspace/microting/eform-angular-frontend/eform-client/e2e/Tests/outer-inner-resource-settings
cp -a Documents/workspace/microting/eform-angular-outer-inner-resource-plugin/eform-client/e2e/Tests/outer-inner-resource-general Documents/workspace/microting/eform-angular-frontend/eform-client/e2e/Tests/outer-inner-resource-general
cp -a Documents/workspace/microting/eform-angular-outer-inner-resource-plugin/eform-client/e2e/Page\ objects/OuterInnerResource Documents/workspace/microting/eform-angular-frontend/eform-client/e2e/Page\ objects/OuterInnerResource
cp -a Documents/workspace/microting/eform-angular-outer-inner-resource-plugin/eform-client/wdio-headless-plugin-step2.conf.js Documents/workspace/microting/eform-angular-frontend/eform-client/wdio-plugin-step2.conf.js
