// dllmain.cpp : Defines the entry point for the DLL application.

#define WIN32_LEAN_AND_MEAN
#ifdef __cplusplus__
#include <cstdlib>
#else
#include <stdlib.h>
#endif
#include "pch.h"
#include "MSCorEE.h"
#include <iostream>
#include <Windows.h>
#include <thread>
#include "vk_code.h"

enum {
	HOTKEY_F4 = 4,
	HOTKEY_F5 = 5,
	HOTKEY_F6 = 6,
	HOTKEY_F9 = 9
};

UINT_PTR unityPlayerBaseAddress = 0;
UINT_PTR unityPlayerOffsetAddress = 0;
UINT_PTR playerAssemblyBaseAddress = 0;

float SavedCoords[3] = { 0,0,0 };
float OldCoords[3] = { 0,0,0 };
bool FPS = false;

void clearConsole() {
	if (system("CLS")) system("clear");
}

void SaveCurrentCoords() {
	if (unityPlayerOffsetAddress == 0) return;
	float X = (*(float*)((*(UINT_PTR*)((*(UINT_PTR*)(unityPlayerOffsetAddress + 0x88)) + 0x8)) + 0xA8));
	float Z = (*(float*)((*(UINT_PTR*)((*(UINT_PTR*)(unityPlayerOffsetAddress + 0x88)) + 0x8)) + 0xA4));
	float Y = (*(float*)((*(UINT_PTR*)((*(UINT_PTR*)(unityPlayerOffsetAddress + 0x88)) + 0x8)) + 0xA0));
	printf("[+] [SAVED] X: %f\n    Y: %f\n    Z: %f\n", X, Y, Z);
	SavedCoords[0] = X;
	SavedCoords[1] = Z;
	SavedCoords[2] = Y;
}
void TELE() {
	if (unityPlayerOffsetAddress == 0) return;
	double counter = 0.001;
	while (counter <= 1.0) {
		if (SavedCoords[0] != 0)
			(*(float*)((*(UINT_PTR*)((*(UINT_PTR*)(unityPlayerOffsetAddress + 0x88)) + 0x8)) + 0xA8)) = SavedCoords[0];
		if (SavedCoords[1] != 0)
			(*(float*)((*(UINT_PTR*)((*(UINT_PTR*)(unityPlayerOffsetAddress + 0x88)) + 0x8)) + 0xA4)) = SavedCoords[1];
		if (SavedCoords[2] != 0)
			(*(float*)((*(UINT_PTR*)((*(UINT_PTR*)(unityPlayerOffsetAddress + 0x88)) + 0x8)) + 0xA0)) = SavedCoords[2];
		Sleep(1);
		counter += 0.001;
	}
}

void ToggleFPS() {
	FPS = !FPS;
	if (FPS)
		*(int*)(unityPlayerBaseAddress + 0x18269C4) = 200;
	else
		*(int*)(unityPlayerBaseAddress + 0x18269C4) = 30;

	printf("[+] FPS toggled: %i\n", *(int*)(unityPlayerBaseAddress + 0x18269C4));
}

void WalkForTeleport() {
	printf("WALKING\n");

	keybd_event(VkKeyScan('W'), 0x9e, 0, 0);
	Sleep(500);
	keybd_event(VkKeyScan('W'), 0x9e, KEYEVENTF_KEYUP, 0);
	keybd_event(VkKeyScan('A'), 0x9e, 0, 0);
	Sleep(500);
	keybd_event(VkKeyScan('A'), 0x9e, KEYEVENTF_KEYUP, 0);
	keybd_event(VkKeyScan('S'), 0x9e, 0, 0);
	Sleep(500);
	keybd_event(VkKeyScan('S'), 0x9e, KEYEVENTF_KEYUP, 0);
	keybd_event(VkKeyScan('D'), 0x9e, 0, 0);
	Sleep(500);
	keybd_event(VkKeyScan('D'), 0x9e, KEYEVENTF_KEYUP, 0);
}
void FreezeTeleport() {
	printf("[+] begin teleporting !! Please wait !!\n");
	TELE();
	printf("[+] finish teleporting\n");
}

void PrintMenu() {
	clearConsole();
	printf("#######################################\n"
		"#  Hotkey  #          Function        #\n"
		"#----------#--------------------------#\n"
		"#    F4    # Save current Coordinates #\n"
		"#    F5    # Teleport to saved Coords #\n"
		"#    F6    # Toggle FPS (30|200)      #\n"
		"#    F9    #       Exit programm      #\n"
		"#######################################\n");
	Sleep(1000);
}

unsigned long main_thread(void*)
{
	if (!AllocConsole())
	{
		return 1;
	}

	freopen_s(reinterpret_cast<FILE**>(stdin), "CONIN$", "r", stdin);
	freopen_s(reinterpret_cast<FILE**>(stdout), "CONOUT$", "w", stdout);
	SetConsoleTitle(TEXT("SiedlerLP | "));
	HMODULE x = LoadLibrary("UnityPlayer.dll");
	unityPlayerBaseAddress = (UINT_PTR)x;
	unityPlayerOffsetAddress = (*(UINT_PTR*)(unityPlayerBaseAddress + 0x1934C10));
	printf("\nHello inside of Genshin Impact.\n\n");
	printf("[+] unityPlayerBaseAddress: 0x%llx\n", unityPlayerBaseAddress);
	printf("[+] unityPlayerOffsetAddress: 0x%llx\n", unityPlayerOffsetAddress);
	bool Continue = true;
	bool block = false;

	RegisterHotKey(NULL, HOTKEY_F4, MOD_NOREPEAT, VK_F4);
	RegisterHotKey(NULL, HOTKEY_F5, MOD_NOREPEAT, VK_F5);
	RegisterHotKey(NULL, HOTKEY_F6, MOD_NOREPEAT, VK_F6);
	RegisterHotKey(NULL, HOTKEY_F9, MOD_NOREPEAT, VK_F9);

	MSG msg = { 0 };
	//std::thread tMenu(PrintMenu);
	//tMenu.join();
	while (GetMessage(&msg, NULL, 0, 0) != 0)
	{
		if (msg.message == WM_HOTKEY)
		{
			switch (msg.wParam)
			{
			case HOTKEY_F4:
				SaveCurrentCoords();
				break;
			case HOTKEY_F5: {
				if (SavedCoords[0] == 0 || SavedCoords[1] == 0 || SavedCoords[2] == 2) printf("[+] No teleport-coordiantes saved\n");
				std::thread t1(FreezeTeleport);
				//std::thread tMove(WalkForTeleport);
				t1.join();
				//tMove.join();
				break;
			}
			case HOTKEY_F6:
				ToggleFPS();
				break;
			case HOTKEY_F9:
				FreeConsole();
				return 0;
				break;
			default:
				break;
			}
		}
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
