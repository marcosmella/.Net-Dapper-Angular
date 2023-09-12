import { Component } from "@angular/core";

import { MenuService } from "./services/menu.service";
@Component({
	selector: "vlfrontend-health-root",
	templateUrl: "./app.component.html",
	styleUrls: ["./app.component.css"]
})
export class AppComponent {
	title = "VLFrontendHealth";
	constructor(public menuServ: MenuService) {}
}
