WebApiAuthAndAzureStorage
=========================

Solution for a todo list of activities, which uses web api with oauth2 authorization, then stores the activities in a azure table storage while logging write actions into an azure queue storage

REQUIRES:
- IIS
- Setting a real azure storage account in the Web.config in the webapi project, or azure storage emulator on running machine
