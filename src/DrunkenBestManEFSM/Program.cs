using DrunkenBestManEFSM.Application.Contracts;
using DrunkenBestManEFSM.Application.Services;
using DrunkenBestManEFSM.Application.Services.Blackjack;
using DrunkenBestManEFSM.Infrastructure.Random;
using DrunkenBestManEFSM.Infrastructure.Xml;
using DrunkenBestManEFSM.Presentation.Console;
using DrunkenBestManEFSM.Presentation.Menus;
using DrunkenBestManEFSM.Presentation.Menus.Blackjack;
using DrunkenBestManEFSM.Presentation.Renderers;
using DrunkenBestManEFSM.Presentation.Renderers.Blackjack;

var textFilePath = Path.Combine(AppContext.BaseDirectory, "Resources", "Texts", "game-texts.xml");

ITextProvider textProvider = new XmlTextProvider(textFilePath);
IRandomProvider randomProvider = new SystemRandomProvider();

var sessionService = new GameSessionService(randomProvider);
var queryService = new GameQueryService(sessionService);
var actionService = new GameActionService(sessionService, queryService, randomProvider);
var blackjackSessionService = new BlackjackSessionService(randomProvider);
var blackjackQueryService = new BlackjackQueryService(blackjackSessionService);
var blackjackActionService = new BlackjackActionService(blackjackSessionService);

var inputReader = new ConsoleInputReader(textProvider);
var printer = new ConsolePrinter();

var statusRenderer = new GameStatusRenderer(printer, textProvider);
var actionsRenderer = new AvailableActionsRenderer(printer, textProvider);
var destinationsRenderer = new AvailableDestinationsRenderer(printer, textProvider);
var actionResultRenderer = new ActionResultRenderer(printer, textProvider, statusRenderer);
var blackjackRenderer = new BlackjackRenderer(printer, textProvider);

var blackjackMenu = new BlackjackMenu(
    inputReader,
    printer,
    textProvider,
    queryService,
    actionService,
    blackjackSessionService,
    blackjackQueryService,
    blackjackActionService,
    blackjackRenderer,
    actionResultRenderer);

var gameMenu = new ConsoleGameMenu(
    inputReader,
    printer,
    textProvider,
    queryService,
    actionService,
    statusRenderer,
    actionsRenderer,
    destinationsRenderer,
    actionResultRenderer,
    blackjackMenu);

var mainMenu = new ConsoleMainMenu(
    inputReader,
    printer,
    textProvider,
    sessionService,
    gameMenu,
    actionResultRenderer);

mainMenu.Run();
