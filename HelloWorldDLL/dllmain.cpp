// dllmain.cpp : Defines the entry point for the DLL application.

#define WIN32_LEAN_AND_MEAN
#include "pch.h"
#include "MSCorEE.h"
#include <iostream>
#include <Windows.h>


unsigned long main_thread(void*)
{
	if (!AllocConsole())
	{
		return 1;
	}

	freopen_s(reinterpret_cast<FILE**>(stdin), "CONIN$", "r", stdin);
	freopen_s(reinterpret_cast<FILE**>(stdout), "CONOUT$", "w", stdout);
	SetConsoleTitle(TEXT("Genshin [Corrupted]"));
	HMODULE x = LoadLibrary("UnityPlayer.dll");
	UINT_PTR unityplayer = (UINT_PTR)x;
	UINT_PTR unityPlayerAddress = (*(UINT_PTR*)(unityplayer + 0x1934C10));
	printf("\nHello inside of Genshin Impact.\n\n");
	printf("[+] unityPlyer: 0x%llx\n", unityplayer);
	printf("[+] unityPlayerAddress: 0x%llx\n", unityPlayerAddress);/*
	printf("[+] first offset 0x88: 0x%llx\n", (*(UINT_PTR*)(unityPlayerAddress+0x88)));
	printf("[+] second offset 0x8: 0x%llx\n", (*(UINT_PTR*)((*(UINT_PTR*)(unityPlayerAddress + 0x88))+0x8)));
	printf("[+] X: %f\n", (*(float*)((*(UINT_PTR*)((*(UINT_PTR*)(unityPlayerAddress + 0x88)) + 0x8)) + 0xA8)));*/
	bool Continue = true;
	bool block = false;
	while (Continue) {
		if (GetKeyState(VK_F4) & 0x8000)
		{
			if (!block) {
				block = true;
				printf("Key pressed\n");
				float X = (*(float*)((*(UINT_PTR*)((*(UINT_PTR*)(unityPlayerAddress + 0x88)) + 0x8)) + 0xA8));
				float Y = (*(float*)((*(UINT_PTR*)((*(UINT_PTR*)(unityPlayerAddress + 0x88)) + 0x8)) + 0xA4));
				float Z = (*(float*)((*(UINT_PTR*)((*(UINT_PTR*)(unityPlayerAddress + 0x88)) + 0x8)) + 0xA0));
				printf("[+] X: %f\n    Y: %f\n    Z: %f\n", X, Y, Z);
			}
			block = false;
		}
		if (GetKeyState(VK_F5) & 0x8000) {
			Continue = false;
			FreeConsole();
		}
		Sleep(500);
	}

	FreeConsole();
	return 0;
}

BOOL APIENTRY DllMain(HMODULE module_handle, DWORD call_reason, LPVOID reserved)
{
	if (call_reason == DLL_PROCESS_ATTACH)
	{
		if (const auto handle = CreateThread(nullptr, 0, &main_thread, nullptr, 0, nullptr); handle != nullptr)
		{
			CloseHandle(handle);
		}

		return TRUE;
	}

	return TRUE;
}
