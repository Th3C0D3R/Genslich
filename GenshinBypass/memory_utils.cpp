#include "memory_utils.hpp"

bool mem_utils::init(bool print)
{
	if (print)
		ENTER_FUNC();

	const uint32_t process_id = process::get_process_id(WINDOW_TITLE);

	if (print)
		printf("[+] process id: %d\n", process_id);

	const HANDLE process_handle = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, FALSE, process_id);

	if (!process_handle || process_handle == INVALID_HANDLE_VALUE)
	{
		if (print)
			printf("[+] failed to obtain process handle\n");
		return false;
	}
	if (print)
		printf("[+] obtained process handle (0x%X) with PROCESS_QUERY_LIMITED_INFORMATION privileage\n",
			process_handle);
	if (print)
		printf("[-] elevating handle (0x%X) ...\n", process_handle);

	if (!grant_access(process_handle, PROCESS_ALL_ACCESS))
	{
		if (print)
			printf("[!] failed to elavate handle (0x%X) GetLastError: 0x%lX\n",
				process_handle, GetLastError());
		return false;
	}
	if (print)
		printf("[+] handle (0x%X) elevated!\n", process_handle);
	if (print)
		printf("[-] obtaining base address...\n");

	const HMODULE base_address = process::get_base_address(process_handle, false);

	if (!base_address)
	{
		if (print)
			printf("[!] failed to obtain base address GetLastError: 0x%lX\n",
				GetLastError());
		return false;
	}
	if (print)
		printf("[+] base address: 0x%llX\n", base_address);

	mem_utils::process_id = process_id;
	mem_utils::process_handle = process_handle;
	mem_utils::base_address = (uint64_t)base_address;
	if (print)
		printf("[+] initialized successfully\n");
	if (print)
		LEAVE_FUNC();

	return true;
}
