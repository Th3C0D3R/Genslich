#include "main.h"

int main(int argc, char** argv)
{
    if (argc < 2)
    {
        printf("[!] incorrect usage.\n[>] usage: %s example.dll\n", argv[0]);
        return 1;
    }

    const char* dll_path = argv[1];

    if (std::string str(dll_path);
        str.substr( str.find_last_of(".") + 1 ) != "dll"
       )
    {
        printf("[!] input file must be a library\n");
        return 1;
    }

    if (!filesystem::is_file_exists(dll_path))
    {
        printf("[!] dll does not exists in \"%s\"\n", dll_path);
        return 1;
    }
    
    int counter = 0;
    while (!mem_utils::init())
    {
        if(counter % 4 == 0)
            printf("[!] failed to initialize memory utility\nIs the Game running?\nIs the Window_Title correct?");
        counter += 1;
        Sleep(1000);
    }

    if (!injector::inject(dll_path))
    {
        printf("[!] failed to inject\n");
        return 1;
    }

    return 0;
}
