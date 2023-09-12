import { Component, OnInit, Input } from "@angular/core";

@Component({
	selector: "app-popover",
	templateUrl: "./popover.component.html",
	styleUrls: ["./popover.component.scss"]
})
export class PopoverComponent implements OnInit {
	@Input() message: string;
	@Input() action: string;
	@Input() positionY: "above" | "below" = "below";
	@Input() positionX: "before" | "after" = "after";
	constructor() {}

	ngOnInit(): void {}
}
