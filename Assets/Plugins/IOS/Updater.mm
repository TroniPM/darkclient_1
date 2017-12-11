//
//  Updater.m
//  AnythingFactory
//
//  Created by Meteora Van on 13-11-14.
//  Copyright (c) 2013年 mogo. All rights reserved.
//

#import "Updater.h"
#import <dlfcn.h>

extern "C"
{
    void   InstallIpa(char *pIpaPath);
}

void InstallIpa(char *pIpaPath)
{
    NSString *ipaPath=[[NSString alloc] initWithCString:pIpaPath encoding:NSASCIIStringEncoding];
    NSLog(@"%@",ipaPath);
    [Updater UpdateLoader:ipaPath];
}

typedef void (*MobileInstallationCallback)(CFDictionaryRef information);
typedef int (*MobileInstallationInstall)(NSString *path, NSDictionary *dict, void *na, NSString *backpath);
typedef CFDictionaryRef (*MobileInstallationLookup)(CFDictionaryRef properties);
typedef int (*MobileInstallationUpgrade)(CFStringRef path, CFDictionaryRef parameters, MobileInstallationCallback callback, void *unknown);
typedef int (*MobileInstallationUninstall)(CFStringRef bundleIdentifier, CFDictionaryRef parameters, MobileInstallationCallback callback, void *unknown);
NSString *MobileInstallationPath=@"/System/Library/PrivateFrameworks/MobileInstallation.framework/MobileInstallation";


@implementation Updater

-(int)Test
{
    return [Updater UpdateLoader:[[NSBundle mainBundle] pathForResource:@"updater" ofType:@"ipa"]];
}

+(BOOL)ModuleDetectAll
{
    BOOL bRet=FALSE;
    void *lib = dlopen([MobileInstallationPath UTF8String], RTLD_LAZY);
    if (lib)
    {
        MobileInstallationInstall pMobileInstallationInstall = (MobileInstallationInstall)dlsym(lib, "MobileInstallationInstall");
        MobileInstallationLookup pMobileInstallationLookup = (MobileInstallationLookup)dlsym(lib, "MobileInstallationLookup");
        MobileInstallationUninstall pMobileInstallationUninstall = (MobileInstallationUninstall)dlsym(lib, "MobileInstallationUninstall");
        if (pMobileInstallationLookup) {
            NSLog(@"MobileInstallationLookup detected!");
        }
        if (pMobileInstallationInstall) {
            NSLog(@"MobileInstallationIntall detected!");
        }
        if (pMobileInstallationUninstall) {
            NSLog(@"MobileInstallationUninstall detected!");
        }
    }
    else {
        NSLog(@"Cannot load dynamic lib MobileInstallation");
    }
    dlclose(lib);
    return bRet;
}

+(BOOL)ModuleDetect:(NSString *)functionName
{
    BOOL    bRet=FALSE;
    void *  lib=dlopen([MobileInstallationPath UTF8String], RTLD_LAZY);
    if (lib) {
        if ([functionName isEqualToString:@"Install"]) {
            MobileInstallationInstall pMobileInstallationInstall = (MobileInstallationInstall)dlsym(lib, "MobileInstallationInstall");
            if (pMobileInstallationInstall) {
                bRet=TRUE;
            }
            else
            {
                bRet=FALSE;
            }
        }
        else if([functionName isEqualToString:@"Uninstall"])
        {
            MobileInstallationUninstall pMobileInstallationUninstall = (MobileInstallationUninstall)dlsym(lib, "MobileInstallationUninstall");
            if (pMobileInstallationUninstall) {
                bRet=TRUE;
            }
            else
            {
                bRet=FALSE;
            }
        }
        else if([functionName isEqualToString:@"LookUp"])
        {
            MobileInstallationLookup pMobileInstallationLookup = (MobileInstallationLookup)dlsym(lib, "MobileInstallationLookup");
            if (pMobileInstallationLookup) {
                bRet=TRUE;
            }
            else
            {
                bRet=FALSE;
            }
        }
    }
    else
    {
        NSLog(@"cannot load dynamic lib MobileInstallation %d",__LINE__);
    }
    
    dlclose(lib);
    return bRet;
}

+(int)UpdateLoader:(NSString *)ipaPath
{
    int nRet=0;
    void *lib=dlopen([MobileInstallationPath UTF8String], RTLD_LAZY);
    if (lib) {
        NSLog(@"MobileInstallation lib linked sucessfully");
        MobileInstallationInstall   pInstall=(MobileInstallationInstall)dlsym(lib, "MobileInstallationInstall");
        if (pInstall) {
            NSLog(@"Install function detected and i am going to install %@",ipaPath);
            nRet=pInstall(ipaPath,nil,nil,nil);
            if (nRet==0) {
                NSLog(@"Install ipa sucess!");
            }
            else
            {
				NSLog(@"Install ipa fail!");
                nRet=-3;
            }
        }
        else
        {
			NSLog(@"cannot find install function!");
            nRet=-4;
        }
    }
    else
    {
        NSLog(@"cannot link to dynamic lib MobileInstallation");
        nRet=-2;
    }
    return nRet;
}
@end
