import { Component, OnInit } from "@angular/core";

@Component({
	selector: "app-empty-route",
	templateUrl: "./empty-route.component.html",
	styleUrls: ["./empty-route.component.scss"]
})
export class EmptyRouteComponent implements OnInit {
	constructor() {}

	ngOnInit(): void {}
}
