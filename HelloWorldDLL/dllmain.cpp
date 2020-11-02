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
#include <vector>
#include "vk_code.h"
#include <fstream>


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

FILE* file;

float SavedCoords[3] = { 0,0,0 };
float OldCoords[3] = { 0,0,0 };
bool FPS = false, ESP = false, IBC = false;
//char* lastLines[5];

void clearConsole();
void addInfoLine(char* newLine);
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
DWORD GetAddressFromSignature(std::vector<int> signature, DWORD startaddress, DWORD endaddress);
void WriteMemory(UINT_PTR address, int value, int length);

unsigned long main_thread(void*)
{
	if (!AllocConsole())
	{
		return 1;
	}
	fopen_s(&file, "genslich.log", "a+");
	freopen_s(reinterpret_cast<FILE**>(stdin), "CONIN$", "r", stdin);
	freopen_s(reinterpret_cast<FILE**>(stdout), "CONOUT$", "w", stdout);
	HMODULE unityPlayerModule = LoadLibrary("UnityPlayer.dll");
	HMODULE userAssemblyModule = LoadLibrary("UserAssembly.dll");
	unityPlayerBaseAddress = (UINT_PTR)unityPlayerModule;
	unityPlayerOffsetAddress = (*(UINT_PTR*)(unityPlayerBaseAddress + 0x1934C10));
	userAssemblyBaseAddress = (UINT_PTR)userAssemblyModule;
	DWORD result = (*(DWORD*)GetAddressFromSignature({ 0x75, 0x69, 0x64, 0x3D},0,0));
	char* name = (char*)(result + 0x4);
	printf("[+] UID: %s\n", name);
	char line[40] = "";
	snprintf(line, sizeof line, "TH3C0D3R(SiedlerLP) | %s\n", name);
	SetConsoleTitle(line);
	printf("[+] unityPlayerBaseAddress: 0x%llx\n", unityPlayerBaseAddress);
	printf("[+] unityPlayerOffsetAddress: 0x%llx\n", unityPlayerOffsetAddress);
	printf("[+] userAssemblyBaseAddress: 0x%llx\n", userAssemblyBaseAddress);
	bool Continue = true;
	bool block = false;

	fprintf_s(file, "Registering Hotkeys....\n");
	fflush(file);

	RegisterHotKey(NULL, HOTKEY_F4, MOD_NOREPEAT, VK_F4);
	RegisterHotKey(NULL, HOTKEY_F5, MOD_NOREPEAT, VK_F5);
	RegisterHotKey(NULL, HOTKEY_F6, MOD_NOREPEAT, VK_F6);
	RegisterHotKey(NULL, HOTKEY_F7, MOD_NOREPEAT, VK_F7);
	RegisterHotKey(NULL, HOTKEY_F8, MOD_NOREPEAT, VK_F8);
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
				//InstantBowCharge();
				break;
			case HOTKEY_F9:
				FreeConsole();
				fclose(file);
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
	if (VirtualProtect(mbi.BaseAddress, mbi.RegionSize, PAGE_EXECUTE_READWRITE, &mbi.Protect))
	{
		memset((void*)address, value, length);
		VirtualProtect(mbi.BaseAddress, mbi.RegionSize, mbi.Protect, &mbi.Protect);
	}
	return;
}

DWORD GetAddressFromSignature(std::vector<int> signature, DWORD startaddress, DWORD endaddress) {
	SYSTEM_INFO si;
	GetSystemInfo(&si);
	if (startaddress == 0)
		startaddress = (DWORD)(si.lpMinimumApplicationAddress);
	if (endaddress == 0)
		endaddress = (DWORD)(si.lpMaximumApplicationAddress);

	MEMORY_BASIC_INFORMATION mbi{ 0 };
	DWORD protectflags = (PAGE_GUARD | PAGE_NOCACHE | PAGE_NOACCESS);
	for (DWORD i = startaddress; i < endaddress - signature.size(); i++) {
		if (VirtualQuery((LPCVOID)i, &mbi, sizeof(mbi))) {
			if (mbi.Protect & protectflags || !(mbi.State & MEM_COMMIT)) {
				i += mbi.RegionSize;
				continue;
			}
			//std::cout << "Good Region! Region Base Address: " << mbi.BaseAddress << " | Region end address: " << std::hex << (int)((DWORD)mbi.BaseAddress + mbi.RegionSize) << std::endl;
			for (DWORD k = (DWORD)mbi.BaseAddress; k < (DWORD)mbi.BaseAddress + mbi.RegionSize - signature.size(); k++) {
				for (DWORD j = 0; j < signature.size(); j++) {
					if (signature.at(j) != -1 && signature.at(j) != *(byte*)(k + j))
						break;
					if (j + 1 == signature.size())
						return k;
				}
			}
			i = (DWORD)mbi.BaseAddress + mbi.RegionSize;
		}
	}
	return NULL;
}

void ESPHack() {
	ESP = !ESP;
	MonsterHP();
	MonsterLevel();
	ChestESP();
	ChestESPDist();
	char line[40] = "";
	snprintf(line, sizeof line, "[+] ESP (Monster & Chest): %s\n", (ESP ? "enabled" : "disabled"));
	printf(line);
	//char line[50];
	//strcpy_s(line, "ESP (Monster & Chest): ");
	//strcat_s(line, (ESP ? "activated" : "disabled"));
	//{
	//	//std::lock_guard<std::mutex> lockGuard(lineChanges);
	//	addInfoLine(line);
	//}
}
void MonsterLevel() {
	WriteMemory(userAssemblyBaseAddress + 0x125AD3E, ESP ? 0x84 : 0x87, 1); //0x8
	printf("MonsterLevel Address: 0x%llx\n", userAssemblyBaseAddress + 0x125AD3E);
}
void MonsterHP() {
	WriteMemory(userAssemblyBaseAddress + 0x12597BB, ESP ? 0x74 : 0x76, 1); //0x76
	printf("MonsterHP Address: 0x%llx\n", userAssemblyBaseAddress + 0x12597BB);
}
void ChestESP() {
	WriteMemory(userAssemblyBaseAddress + 0x1C6F317, ESP ? 0x75 : 0x74, 1); //0x74
	printf("ChestESP Address: 0x%llx\n", userAssemblyBaseAddress + 0x1C6F317);
}
void ChestESPDist() {
	WriteMemory(userAssemblyBaseAddress + 0x1C6F39A, ESP ? 0x75 : 0x74, 1); //0x74
	printf("ChestDistESP Address: 0x%llx\n", userAssemblyBaseAddress + 0x1C6F39A);
}
void InstantBowCharge() {
	IBC = !IBC;
	if (IBC)
		WriteMemory(userAssemblyBaseAddress + 0x346DCCF, 0x90, 4); //0F 11 47 10
	else {
		WriteMemory(userAssemblyBaseAddress + 0x346DCCF, 0x0F, 1);
		WriteMemory(userAssemblyBaseAddress + 0x346DCD0, 0x11, 1);
		WriteMemory(userAssemblyBaseAddress + 0x346DCD1, 0x47, 1);
		WriteMemory(userAssemblyBaseAddress + 0x346DCD2, 0x10, 1);
	}
	char line[35] = "";
	snprintf(line, sizeof line, "[+] InstantBowCharge: %s\n", (IBC ? "enabled" : "disabled"));
	printf(line);
	//char line[50];
	//strcpy_s(line, "InstantBowCharge: ");
	//strcat_s(line, (IBC ? "activated" : "disabled"));
	//{
	//	//std::lock_guard<std::mutex> lockGuard(lineChanges);
	//	addInfoLine(line);
	//}
}
void clearConsole() {
	if (system("CLS")) system("clear");
}
void SaveCurrentCoords() {
	if (unityPlayerOffsetAddress == 0) return;
	float X = (*(float*)((*(UINT_PTR*)((*(UINT_PTR*)(unityPlayerOffsetAddress + 0x88)) + 0x8)) + 0xA8));
	float Z = (*(float*)((*(UINT_PTR*)((*(UINT_PTR*)(unityPlayerOffsetAddress + 0x88)) + 0x8)) + 0xA4));
	float Y = (*(float*)((*(UINT_PTR*)((*(UINT_PTR*)(unityPlayerOffsetAddress + 0x88)) + 0x8)) + 0xA0));
	char line[UCHAR_MAX] = "";
	snprintf(line, sizeof line, "[+] [SAVED]\n X: %f\n Y: %f\n Z: %f\n", X, Y, Z);
	printf(line);
	SavedCoords[0] = X;
	SavedCoords[1] = Z;
	SavedCoords[2] = Y;
	//char line[120];
	//char Coord[sizeof SavedCoords[0]];
	//strcpy_s(line, "Saved Coords: ");
	//_gcvt_s(Coord, sizeof(SavedCoords[0]), SavedCoords[0], 8);
	//strcat_s(line, Coord);
	//strcat_s(line, " | ");
	//_gcvt_s(Coord, sizeof(SavedCoords[2]), SavedCoords[2], 8);
	//strcat_s(line, Coord);
	//strcat_s(line, " | ");
	//_gcvt_s(Coord, sizeof(SavedCoords[1]), SavedCoords[1], 8);
	//strcat_s(line, Coord);
	//{
	//	//std::lock_guard<std::mutex> lockGuard(lineChanges);
	//	addInfoLine(line);
	//}
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
	char line[UCHAR_MAX] = "";
	snprintf(line, sizeof line, "[+] Teleport to: %f | %f | %f\n", SavedCoords[0], SavedCoords[2], SavedCoords[1]);
	printf(line);
	//char line[120];
	//char Coord[sizeof SavedCoords[0]];
	//strcpy_s(line, "Teleporting to: ");
	//_gcvt_s(Coord, sizeof(SavedCoords[0]), SavedCoords[0], 8);
	//strcat_s(line, Coord);
	//strcat_s(line, " | ");
	//_gcvt_s(Coord, sizeof(SavedCoords[2]), SavedCoords[2], 8);
	//strcat_s(line, Coord);
	//strcat_s(line, " | ");
	//_gcvt_s(Coord, sizeof(SavedCoords[1]), SavedCoords[1], 8);
	//strcat_s(line, Coord);
	//{
	//	//std::lock_guard<std::mutex> lockGuard(lineChanges);
	//	addInfoLine(line);
	//}
}
void ToggleFPS() {
	FPS = !FPS;
	if (FPS)
		*(int*)(unityPlayerBaseAddress + 0x18269C4) = 200;
	else
		*(int*)(unityPlayerBaseAddress + 0x18269C4) = 30;

	char line[UCHAR_MAX] = "";
	snprintf(line, sizeof line, "[+] FPS toggled: %i\n", *(int*)(unityPlayerBaseAddress + 0x18269C4));
	printf(line);

	//char line[50];
	//strcpy_s(line, sizeof line, "FPS toggled: ");
	//strcat_s(line, sizeof line, (char*)*(int*)(unityPlayerBaseAddress + 0x18269C4));
	//{
	//	//std::lock_guard<std::mutex> lockGuard(lineChanges);
	//	addInfoLine(line);
	//}
}
void WalkForTeleport() {
	keybd_event(VK_SPACE, 0x9e, 0, 0);
	Sleep(250);
	keybd_event(VK_SPACE, 0x9e, KEYEVENTF_KEYUP, 0);
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
		"\n");
	/*for (int i = 4; i >= 0; i--)
	{
		printf(lastLines[i]);
		printf("\n");
	}*/
	Sleep(200);
}
void addInfoLine(char* newLine) {
	//std::lock_guard<std::mutex> lockGuard(mlastLines);
	/*for (int i = 0; i < sizeof lastLines; i++)
	{
		if (i == (sizeof lastLines) - 1) lastLines[i] = newLine;
		else lastLines[i] = lastLines[i + 1];
	}*/
}

BOOL APIENTRY DllMain(HMODULE module_handle, DWORD call_reason, LPVOID reserved)
{
	if (call_reason == DLL_PROCESS_ATTACH)
	{
		//if (const auto handle = CreateThread(nullptr, 0, &CreateDotNetRunTime, nullptr, 0, nullptr); handle != nullptr)
		if (const auto handle = CreateThread(nullptr, 0, &main_thread, nullptr, 0, nullptr); handle != nullptr)
			CloseHandle(handle);
		return TRUE;
	}
	return TRUE;
}
