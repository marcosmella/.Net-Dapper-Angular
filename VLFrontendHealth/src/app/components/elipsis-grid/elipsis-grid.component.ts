import { Component, OnInit, Input } from "@angular/core";

@Component({
	selector: "app-elipsis-grid",
	templateUrl: "./elipsis-grid.component.html",
	styleUrls: ["./elipsis-grid.component.scss"]
})
export class ElipsisGridComponent implements OnInit {
	@Input() actions: ElipsisAction[];
	@Input() element: any;

	public selected: Boolean = false;

	constructor() {}

	ngOnInit(): void {}

	condition(action: ElipsisAction): Boolean {
		return action.condition ? action.condition(this.element) : true;
	}
}

export enum IconTypes {
	delete = "delete",
	edit = "edit",
	update = "update",
	search = "search",
	view = "view",
	duplicate = "duplicate",
	open = "open",
	import = "import",
	active = "active",
	inactive = "inactive",
	download = "download",
	upload = "upload",
	absences = "absences",
	document = "document",
	follow = "follow",
	complaint = "complaint"
}

export interface ElipsisAction {
	description?: string;
	icon: IconTypes;
	action(element: any): any;
	condition?(element: any): boolean;
}
