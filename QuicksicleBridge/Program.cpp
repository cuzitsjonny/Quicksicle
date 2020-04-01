#include <cstring>

#include "RakPeerInterface.h"
#include "RakNetworkFactory.h"
#include "RakNetTypes.h"
#include "PacketPriority.h"
#include "MessageIdentifiers.h"
#include "RakNetDefines.h"
#include "Export.h"
#include "RakSleep.h"

using namespace System;
using namespace System::IO;
using namespace System::Collections::Generic;
using namespace System::Runtime::InteropServices;

// Creates a managed copy of an unmanaged string.
String^ to_managed_string(const char* unmanagedStr)
{
	String^ managedStr = String::Empty;

	for (size_t i = 0; i < strlen(unmanagedStr); i++)
	{
		managedStr += (Char)unmanagedStr[i];
	}

	return managedStr;
}

// Creates an unmanaged copy of a managed string.
const char* to_unmanaged_string(String^ managedStr)
{
	char* unmanagedStr = new char[managedStr->Length + 1];

	for (int i = 0; i < managedStr->Length; i++)
	{
		unmanagedStr[i] = (char)managedStr[i];
	}

	unmanagedStr[managedStr->Length] = 0;

	return unmanagedStr;
}

// Creates a managed copy of an unmanaged array.
array<Byte>^ to_managed_array(unsigned char* unmanagedArray, int unmanagedLength)
{
	array<Byte>^ managedArray = gcnew array<Byte>(unmanagedLength);

	for (int i = 0; i < unmanagedLength; i++)
	{
		managedArray[i] = unmanagedArray[i];
	}

	return managedArray;
}

// Creates an unmanaged copy of a managed array.
unsigned char* to_unmanaged_array(array<Byte>^ managedArray, int* unmanagedLength)
{
	unsigned char* unmanagedArray = new unsigned char[managedArray->Length];

	for (int i = 0; i < managedArray->Length; i++)
	{
		unmanagedArray[i] = managedArray[i];
	}

	*unmanagedLength = managedArray->Length;

	return unmanagedArray;
}

// This is where the fun begins.
int main(array<String^>^ args)
{
	// Possible startup arguments and their default values.
	String^ loggerDirectoryPath = "./logs/";
	String^ loggerDateTimeFormat = "yyyy-MM-dd HH-mm-ss";
	int loggerFileSplitPeriod = 180;
	int rakPeerMaxIncomingConnections = 64;
	int rakPeerSleepTimer = 30;
	String^ mySqlHost = "localhost";
	String^ mySqlUser = "quicksicle";
	String^ mySqlPassword = "password";
	String^ mySqlDatabase = "quicksicle";
	int mySqlPort = 3306;
	int sqlitePoolSize = 4;
	unsigned short authPort = 1001;
	unsigned short worldPort = 1002;
	String^ resourcesDirectoryPath = "./res/";

	// Temporary variable to hold the argument key that is currently being processed.
	String^ argsKey = nullptr;

	// Argument processing.
	for (int i = 0; i < args->Length; i++)
	{
		String^ argument = args[i];

		if (argument->StartsWith("-"))
		{
			argsKey = argument;
		}
		else
		{
			if (argsKey != nullptr)
			{
				if (argsKey->Equals("-loggerDirectoryPath", StringComparison::InvariantCultureIgnoreCase))
				{
					loggerDirectoryPath = argument;
				}
				else if (argsKey->Equals("-loggerDateTimeFormat", StringComparison::InvariantCultureIgnoreCase))
				{
					loggerDateTimeFormat = argument;
				}
				else if (argsKey->Equals("-loggerFileSplitPeriod", StringComparison::InvariantCultureIgnoreCase))
				{
					int output;

					if (Int32::TryParse(argument, output))
					{
						loggerFileSplitPeriod = output;
					}
				}
				else if (argsKey->Equals("-rakPeerMaxIncomingConnections", StringComparison::InvariantCultureIgnoreCase))
				{
					int output;

					if (Int32::TryParse(argument, output))
					{
						rakPeerMaxIncomingConnections = output;
					}
				}
				else if (argsKey->Equals("-rakPeerSleepTimer", StringComparison::InvariantCultureIgnoreCase))
				{
					int output;

					if (Int32::TryParse(argument, output))
					{
						rakPeerSleepTimer = output;
					}
				}
				else if (argsKey->Equals("-mySqlHost", StringComparison::InvariantCultureIgnoreCase))
				{
					mySqlHost = argument;
				}
				else if (argsKey->Equals("-mySqlUser", StringComparison::InvariantCultureIgnoreCase))
				{
					mySqlUser = argument;
				}
				else if (argsKey->Equals("-mySqlPassword", StringComparison::InvariantCultureIgnoreCase))
				{
					mySqlPassword = argument;
				}
				else if (argsKey->Equals("-mySqlDatabase", StringComparison::InvariantCultureIgnoreCase))
				{
					mySqlDatabase = argument;
				}
				else if (argsKey->Equals("-mySqlPort", StringComparison::InvariantCultureIgnoreCase))
				{
					int output;

					if (Int32::TryParse(argument, output))
					{
						mySqlPort = output;
					}
				}
				else if (argsKey->Equals("-sqlitePoolSize", StringComparison::InvariantCultureIgnoreCase))
				{
					int output;

					if (Int32::TryParse(argument, output))
					{
						sqlitePoolSize = output;
					}
				}
				else if (argsKey->Equals("-authPort", StringComparison::InvariantCultureIgnoreCase))
				{
					unsigned short output;

					if (UInt16::TryParse(argument, output))
					{
						authPort = output;
					}
				}
				else if (argsKey->Equals("-worldPort", StringComparison::InvariantCultureIgnoreCase))
				{
					unsigned short output;

					if (UInt16::TryParse(argument, output))
					{
						worldPort = output;
					}
				}
				else if (argsKey->Equals("-resourcesDirectoryPath", StringComparison::InvariantCultureIgnoreCase))
				{
					resourcesDirectoryPath = argument;
				}
			}
		}
	}

	argsKey = nullptr;

	Console::WriteLine("Effective startup arguments:");
	Console::WriteLine(" loggerDirectoryPath           => " + Path::GetFullPath(loggerDirectoryPath));
	Console::WriteLine(" resourcesDirectoryPath        => " + Path::GetFullPath(resourcesDirectoryPath));
	Console::WriteLine(" loggerDateTimeFormat          => " + loggerDateTimeFormat);
	Console::WriteLine(" loggerFileSplitPeriod         => " + loggerFileSplitPeriod);
	Console::WriteLine(" rakPeerMaxIncomingConnections => " + rakPeerMaxIncomingConnections);
	Console::WriteLine(" rakPeerSleepTimer             => " + rakPeerSleepTimer);
	Console::WriteLine(" mySqlHost                     => " + mySqlHost);
	Console::WriteLine(" mySqlUser                     => " + mySqlUser);
	Console::WriteLine(" mySqlPassword                 => " + mySqlPassword);
	Console::WriteLine(" mySqlDatabase                 => " + mySqlDatabase);
	Console::WriteLine(" mySqlPort                     => " + mySqlPort);
	Console::WriteLine(" sqlitePoolSize                => " + sqlitePoolSize);
	Console::WriteLine(" authPort                      => " + authPort);
	Console::WriteLine(" worldPort                     => " + worldPort);
	Console::WriteLine("If you think something is wrong about these, check your command line input.");
	Console::WriteLine();
	Console::WriteLine("Switching to logger.");
	Console::WriteLine();

	// Creation of the logger. Every cool app has a logger.
	Quicksicle::Logging::Logger^ logger = gcnew Quicksicle::Logging::Logger(loggerDirectoryPath, loggerDateTimeFormat, loggerFileSplitPeriod);

	// Creation of a few other things needed.
	Quicksicle::Database::DatabaseManager^ databaseManager = gcnew Quicksicle::Database::DatabaseManager(mySqlHost, mySqlPort, mySqlUser, mySqlPassword, mySqlDatabase, resourcesDirectoryPath, sqlitePoolSize);
	Quicksicle::Resources::PredefinedNameCache^ predefinedNameCache;
	List<Quicksicle::Resources::Zone^>^ zones = gcnew List<Quicksicle::Resources::Zone^>();

	try
	{
		predefinedNameCache = gcnew Quicksicle::Resources::PredefinedNameCache(resourcesDirectoryPath);

		logger->Log("Loaded PredefinedNameCache.");
	}
	catch (Exception^ exc)
	{
		logger->Log("A loading error (PredefinedNameCache) has occured. exception=" + exc->Message);
	}

	array<String^>^ filePaths = Directory::GetFiles(resourcesDirectoryPath);

	for (int i = 0; i < filePaths->Length; i++)
	{
		String^ filePath = filePaths[i];

		if (filePath->ToUpper()->EndsWith(".LUZ"))
		{
			Quicksicle::Resources::Zone^ zone = gcnew Quicksicle::Resources::Zone(filePath);

			zones->Add(zone);

			logger->Log("Loaded zone " + zone->WorldId + ".");
		}
	}

	// Since the logger synchronizes console and file writes using an internal thread, we have to explicitly start it.
	logger->Start();

	// Creation of the peer.
	RakPeerInterface* peer = RakNetworkFactory::GetRakPeerInterface();
	SocketDescriptor* socketDescriptors = new SocketDescriptor[2];

	socketDescriptors[0] = SocketDescriptor(authPort, 0);
	socketDescriptors[1] = SocketDescriptor(worldPort, 0);

	peer->InitializeSecurity(0, 0, 0, 0);

	// Binding the peer to the specified ports.
	bool success = peer->Startup(rakPeerMaxIncomingConnections, rakPeerSleepTimer, socketDescriptors, 2);

	if (success)
	{
		logger->Log("Bound the peer to the UDP ports " + authPort + " and " + worldPort + ".");

		// Since the peer's Startup method only sets how many peer connections are possible, we have to configure how many of those can be incoming connections.
		peer->SetMaximumIncomingConnections(rakPeerMaxIncomingConnections);

		// This is hardcoded into the LEGO Universe client, so we don't have much of a choice for our incoming password.
		peer->SetIncomingPassword("3.25 ND1", 8);

		logger->Log("Finished peer configuration.");

		// Creation of the actual server. This emulator does not care about how LEGO Universe worked in the past.
		// It handles authentication, character selection, and world activity. I'm confident this will work out.
		// If not, I'll curse the day I wrote this comment.
		Quicksicle::Core::Server^ server = gcnew Quicksicle::Core::Server(authPort, worldPort, logger, databaseManager, predefinedNameCache, zones);

		// Starting the server. The packet processor, the scheduler, and the event manager have internal threads that need to be start.
		server->Start();

		bool run = true;

		while (run)
		{
			bool canSleep = true;

			// Check for incoming packets.
			Packet* incomingPacket = peer->Receive();

			if (incomingPacket != nullptr)
			{
				// A packet is received from the RakNet peer
				// and enqueued into the incoming packets
				// queue of the Quicksicle server.
				SystemAddress sourceSystemAddress = incomingPacket->systemAddress;
				const char* unmanagedSourceAddress = sourceSystemAddress.ToString(false);
				String^ managedSourceAddress = to_managed_string(unmanagedSourceAddress);

				SystemAddress destinationSystemAddress = peer->GetExternalID(sourceSystemAddress);
				const char* unmanagedDestinationAddress = destinationSystemAddress.ToString(false);
				String^ managedDestinationAddress = to_managed_string(unmanagedDestinationAddress);

				array<Byte>^ data = to_managed_array(incomingPacket->data, incomingPacket->length);

				server->IncomingPacketQueue->Enqueue(gcnew Quicksicle::Net::IncomingDatagramPacket(data, managedSourceAddress, sourceSystemAddress.port, managedDestinationAddress, destinationSystemAddress.port));
				peer->DeallocatePacket(incomingPacket);

				canSleep = false;
			}

			// Check for outgoing packets.
			Quicksicle::Net::OutgoingDatagramPacket^ outgoingPacket;

			if (server->OutgoingPacketQueue->TryDequeue(outgoingPacket))
			{
				// A packet is dequeued from the outgoing
				// packet queue of the Quicksicle server and
				// sent over the RakNet peer.
				SystemAddress systemAddress;
				String^ managedAddress = outgoingPacket->DestinationAddress;
				const char* unmanagedAddress = to_unmanaged_string(managedAddress);

				systemAddress.SetBinaryAddress(unmanagedAddress);
				systemAddress.port = outgoingPacket->DestinationPort;

				int dataLength = 0;
				unsigned char* data = to_unmanaged_array(outgoingPacket->Data, &dataLength);

				peer->Send((char*)data, dataLength, PacketPriority::SYSTEM_PRIORITY, PacketReliability::RELIABLE_ORDERED, 0, systemAddress, false);

				delete unmanagedAddress;
				delete data;

				canSleep = false;
			}

			if (canSleep)
			{
				RakSleep(rakPeerSleepTimer);
			}
		}

		// Stopping the server.
		server->ShutDown();
	}
	else
	{
		logger->Log("Failed to bind the peer to the UDP ports " + authPort + " and " + worldPort + ". Check if these ports are occupied and if your system supports multiple datagram sockets on the same port.");
	}

	// Destruction of the peer.
	RakNetworkFactory::DestroyRakPeerInterface(peer);

	// Of course the logger's internal thread has to be terminated explicitly as well.
	logger->Terminate();

	Console::WriteLine();
	Console::WriteLine("Press any key to close the application...");
	Console::ReadKey();
	return 0;
}
