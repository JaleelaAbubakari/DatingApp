import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/member';
import { Pagination } from '../_models/pagination';
import { MembersService } from '../_services/members.service';

@Component({
  selector: 'app-visits',
  templateUrl: './visits.component.html',
  styleUrls: ['./visits.component.css']
})
export class VisitsComponent implements OnInit {
  members: Partial<Member[]>;
  predicate = "visited";
  pageNumber = 1;
  pageSize = 5;
  pagination: Pagination;
  visitParams: import("c:/Users/Jaleela Abu/DatingApp_S18/client/src/app/_models/userParams").UserParams;
  

  constructor(private memberService: MembersService) { }

  ngOnInit(): void {
    this.loadVisits();
  }

  loadVisits() {
    this.memberService.getVisits(this.predicate, this.pageNumber, this.pageSize).subscribe(response => {
      this.members = response.result;
      this.pagination = response.pagination;
    })
  }
  pageChanged(event: any) {
    this.pageNumber = event.page;
    this.loadVisits();
  }

   resetFilters() {
     this.visitParams = this.memberService.resetUserParams();
     this.loadVisits();
   }

}
