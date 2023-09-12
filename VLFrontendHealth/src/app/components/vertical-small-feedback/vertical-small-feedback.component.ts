import { Component, OnInit, Input } from "@angular/core";

@Component({
	selector: "app-vertical-small-feedback",
	templateUrl: "./vertical-small-feedback.component.html",
	styleUrls: ["./vertical-small-feedback.component.css"]
})
export class VerticalSmallFeedbackComponent implements OnInit {
	@Input() description: string;
	@Input() type: "noNotes" | "empty";
	public statusDictionary = {
		noNotes: "assets/svg/message/action/icon-fb-note.svg",
		empty: "assets/svg/message/status/icon-fb-empty.svg"
	};
	constructor() {}

	ngOnInit(): void {}
}
