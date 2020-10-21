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
#include <sstream>
#include <vector>
#include <Windows.h>
#include <mutex>
#include <thread>
#include "vk_code.h"

enum {
	HOTKEY_F4 = 4,
	HOTKEY_F5 = 5,
	HOTKEY_F6 = 6,
	HOTKEY_F7 = 7,
	HOTKEY_F8 = 8,
	HOTKEY_F9 = 9
};

UINT_PTR unityPlayerBaseAddress = 0;
UINT_PTR unityPlayerOffsetAddress = 0;
UINT_PTR userAssemblyBaseAddress = 0;

float SavedCoords[3] = { 0,0,0 };
float OldCoords[3] = { 0,0,0 };
bool FPS = false, ESP = false, IBC = false;
std::string lastLines[5] = { "","","","","" };
std::mutex lineChanges, mlastLines;

void clearConsole();
void addInfoLine(std::string newLine);
void SaveCurrentCoords();
void TELE();
void ToggleFPS();
void WalkForTeleport();
void FreezeTeleport();
void PrintMenu();
void ESPHack();
void MonsterLevel();
void MonsterHP();
void ChestESP();
void ChestESPDist();
void InstantBowCharge();
void WriteMemory(UINT_PTR address, int value, int length);

unsigned long main_thread(void*)
{
	if (!AllocConsole())
	{
		return 1;
	}

	freopen_s(reinterpret_cast<FILE**>(stdin), "CONIN$", "r", stdin);
	freopen_s(reinterpret_cast<FILE**>(stdout), "CONOUT$", "w", stdout);
	SetConsoleTitle(TEXT("SiedlerLP | "));
	HMODULE unityPlayerModule = LoadLibrary("UnityPlayer.dll");
	HMODULE userAssemblyModule = LoadLibrary("UserAssembly.dll");
	unityPlayerBaseAddress = (UINT_PTR)unityPlayerModule;
	unityPlayerOffsetAddress = (*(UINT_PTR*)(unityPlayerBaseAddress + 0x1934C10));
	userAssemblyBaseAddress = (UINT_PTR)userAssemblyModule;
	printf("[+] unityPlayerBaseAddress: 0x%llx\n", unityPlayerBaseAddress);
	printf("[+] unityPlayerOffsetAddress: 0x%llx\n", unityPlayerOffsetAddress);
	printf("[+] userAssemblyBaseAddress: 0x%llx\n", userAssemblyBaseAddress);
	bool Continue = true;
	bool block = false;

	RegisterHotKey(NULL, HOTKEY_F4, MOD_NOREPEAT, VK_F4);
	RegisterHotKey(NULL, HOTKEY_F5, MOD_NOREPEAT, VK_F5);
	RegisterHotKey(NULL, HOTKEY_F6, MOD_NOREPEAT, VK_F6);
	RegisterHotKey(NULL, HOTKEY_F7, MOD_NOREPEAT, VK_F7);
	RegisterHotKey(NULL, HOTKEY_F8, MOD_NOREPEAT, VK_F8);
	RegisterHotKey(NULL, HOTKEY_F9, MOD_NOREPEAT, VK_F9);

	MSG msg = { 0 };
	std::thread tMenu(PrintMenu);
	tMenu.join();
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
				std::thread tMove(WalkForTeleport);
				t1.join();
				tMove.join();
				break;
			}
			case HOTKEY_F6:
				ToggleFPS();
				break;
			case HOTKEY_F7:
				ESPHack();
				break;
			case HOTKEY_F8:
				InstantBowCharge();
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

void WriteMemory(UINT_PTR address, int value, int length) {
	MEMORY_BASIC_INFORMATION mbi;
	VirtualQuery((void*)address, &mbi, sizeof(mbi));
	VirtualProtect(mbi.BaseAddress, mbi.RegionSize, PAGE_EXECUTE_READWRITE, &mbi.Protect);
	memset((void*)address, value, length);
	VirtualProtect(mbi.BaseAddress, mbi.RegionSize, mbi.Protect, &mbi.Protect);
	return;
}
void ESPHack() {
	ESP = !ESP;
	MonsterHP();
	MonsterLevel();
	ChestESP();
	ChestESPDist();

	std::ostringstream line;
	line << "ESP (Monster & Chest): " << ESP ? "activated" : "disabled";
	{
		std::lock_guard<std::mutex> lockGuard(lineChanges);
		addInfoLine(line.str());
	}
}
void MonsterLevel() {

	WriteMemory(userAssemblyBaseAddress + 0x125A79E, ESP ? 0x84 : 0x87, 1); //0x87
}
void MonsterHP() {
	WriteMemory(userAssemblyBaseAddress + 0x125921B, ESP ? 0x74 : 0x76, 1); //0x76
}
void ChestESP() {
	WriteMemory(userAssemblyBaseAddress + 0x1C6ED77, ESP ? 0x75 : 0x74, 1); //0x74
}
void ChestESPDist() {
	WriteMemory(userAssemblyBaseAddress + 0x1C6EDFA, ESP ? 0x75 : 0x74, 1); //0x74
}
void InstantBowCharge() {
	IBC = !IBC;
	if (IBC)
		WriteMemory(userAssemblyBaseAddress + 0x348E32F, 0x90, 4); //0F 11 47 10
	else {
		WriteMemory(userAssemblyBaseAddress + 0x348E32F, 0x0F, 1);
		WriteMemory(userAssemblyBaseAddress + 0x348E330, 0x11, 1);
		WriteMemory(userAssemblyBaseAddress + 0x348E331, 0x47, 1);
		WriteMemory(userAssemblyBaseAddress + 0x348E332, 0x10, 1);
	}
	std::ostringstream line;
	line << "InstantBowCharge: " << IBC ? "activated" : "disabled";
	{
		std::lock_guard<std::mutex> lockGuard(lineChanges);
		addInfoLine(line.str());
	}
}
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

	std::ostringstream line;
	line << "Saved Coords " << SavedCoords[0] << " | " << SavedCoords[2] << " | " << SavedCoords[1];
	{
		std::lock_guard<std::mutex> lockGuard(lineChanges);
		addInfoLine(line.str());
	}
}
void TELE() {
	if (unityPlayerOffsetAddress == 0) return;
	double counter = 0.02;
	while (counter <= 1.0) {
		if (SavedCoords[0] != 0)
			(*(float*)((*(UINT_PTR*)((*(UINT_PTR*)(unityPlayerOffsetAddress + 0x88)) + 0x8)) + 0xA8)) = SavedCoords[0];
		if (SavedCoords[1] != 0)
			(*(float*)((*(UINT_PTR*)((*(UINT_PTR*)(unityPlayerOffsetAddress + 0x88)) + 0x8)) + 0xA4)) = SavedCoords[1];
		if (SavedCoords[2] != 0)
			(*(float*)((*(UINT_PTR*)((*(UINT_PTR*)(unityPlayerOffsetAddress + 0x88)) + 0x8)) + 0xA0)) = SavedCoords[2];
		Sleep(1);
		counter += 0.02;
	}
	std::ostringstream line;
	line << "Teleporting to " << SavedCoords[0] << " | " << SavedCoords[2] << " | " << SavedCoords[1];
	{
		std::lock_guard<std::mutex> lockGuard(lineChanges);
		addInfoLine(line.str());
	}
}
void ToggleFPS() {
	FPS = !FPS;
	if (FPS)
		*(int*)(unityPlayerBaseAddress + 0x18269C4) = 200;
	else
		*(int*)(unityPlayerBaseAddress + 0x18269C4) = 30;


	std::ostringstream line;
	line << "FPS toggled: " << *(int*)(unityPlayerBaseAddress + 0x18269C4);
	{
		std::lock_guard<std::mutex> lockGuard(lineChanges);
		addInfoLine(line.str());
	}
}
void WalkForTeleport() {
	keybd_event(VK_SPACE, 0x9e, 0, 0);
	Sleep(250);
	keybd_event(VK_SPACE, 0x9e, KEYEVENTF_KEYUP, 0);
	/*keybd_event(VkKeyScan('A'), 0x9e, 0, 0);
	Sleep(500);
	keybd_event(VkKeyScan('A'), 0x9e, KEYEVENTF_KEYUP, 0);
	keybd_event(VkKeyScan('S'), 0x9e, 0, 0);
	Sleep(500);
	keybd_event(VkKeyScan('S'), 0x9e, KEYEVENTF_KEYUP, 0);
	keybd_event(VkKeyScan('D'), 0x9e, 0, 0);
	Sleep(500);
	keybd_event(VkKeyScan('D'), 0x9e, KEYEVENTF_KEYUP, 0);*/
}
void FreezeTeleport() {
	TELE();
}
void PrintMenu() {
	clearConsole();
	printf("[+] unityPlayerBaseAddress: 0x%llx\n", unityPlayerBaseAddress);
	printf("[+] unityPlayerOffsetAddress: 0x%llx\n", unityPlayerOffsetAddress);
	printf("[+] userAssemblyBaseAddress: 0x%llx\n", userAssemblyBaseAddress);
	printf("\n"
		"#######################################\n"
		"#  Hotkey  #          Function        #\n"
		"#----------#--------------------------#\n"
		"#    F4    # Save current Coordinates #\n"
		"#    F5    # Teleport to saved Coords #\n"
		"#    F6    # Toggle FPS (30|200)      #\n"
		"#    F7    # Toggle ESP               #\n"
		"#    F8    # Toggle InstantBowCharge  #\n"
		"#    F9    #       Exit programm      #\n"
		"#######################################\n"
		"\n"
		"\n"
		"\n"
		"Last actions:\n");
	for (int i = 4; i >= 0; i--)
	{
		printf(lastLines[i].c_str());
		printf("\n");
	}
	Sleep(200);
}
void addInfoLine(std::string newLine) {
	std::lock_guard<std::mutex> lockGuard(mlastLines);
	for (int i = 0; i < sizeof lastLines; i++)
	{
		if (i == (sizeof lastLines) - 1) lastLines[i] = newLine;
		else lastLines[i] = lastLines[i+1];
	}
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
