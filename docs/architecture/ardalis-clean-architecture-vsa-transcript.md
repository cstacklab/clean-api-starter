# Transcript — Ardalis on Clean Architecture vs. Vertical Slices

> **Attribution / source.** This is a lightly edited transcript of a talk by
> **Steve Smith (Ardalis)** responding to a video by Nick Chapsas about .NET
> practices he no longer recommends. It is reproduced here only as a reference
> for the architectural decisions recorded in
> [`clean-architecture-and-vertical-slices.md`](clean-architecture-and-vertical-slices.md).
>
> The text was auto-transcribed and then lightly cleaned for readability;
> wording is approximate and may contain transcription errors. **Prefer linking
> to the original video** rather than redistributing this transcript — if this
> repository is public, consider replacing the body below with a link and a short
> summary to respect the author's copyright.

---

## On the "six-project solution" criticism

I recently watched a video from Nick Chapsas about software practices he used to
recommend for .NET developers that he doesn't really use anymore, and one of the
first topics was Clean Architecture. I actually agree with a lot of what Nick
said — especially that a giant six-project solution is probably not the right
default for many applications today. (Granted, Microsoft is responsible for a
third of those projects now, due to Aspire.) A six-project solution is not a
prerequisite for good architecture — which is slightly awkward for me to admit,
since I maintain one of the more popular Clean Architecture templates in the .NET
ecosystem. More than one, it turns out.

The industry has evolved, technology has evolved, and that's a good thing.

## Clean Architecture and Vertical Slices are not opposites

One thing I want to clarify: **Vertical Slice Architecture and Clean Architecture
are not opposites. They solve different problems.** Many people confuse Clean
Architecture with a specific folder structure from around 2018. Those are not the
same thing.

To me, Clean Architecture is fundamentally about **dependency direction**. It's
about protecting your business logic from external dependencies, isolating
infrastructure, encapsulation, and enforcing boundaries. It is *not* about
whether you have one, two, or twelve projects.

And separately from that, **you should build in vertical slices.** I've been
advocating for feature-based organization for over a decade — I wrote about
vertical slices and vertical features for ASP.NET on DevIQ around 2015, and in an
MSDN magazine article around 2016, a couple of years before Jimmy Bogard's
vertical slice architecture article.

So when Nick says applications are easier to understand when organized around
features — yes, absolutely. If I need to add a feature related to orders, I should
navigate to the orders feature set and add it there, with little need to look
elsewhere. That part is not controversial.

## Where older layered architectures went wrong

Early layered architectures put *all* data access in one layer, *all* business
logic in another, and the UI in a third. With MVC, the UI layer also had root
folders for controllers, models, view models, and views. So every time you wanted
to add a feature, you had to add files in every one of these layers and folders.
The constant scrolling around the directory structure added a lot of unnecessary
friction. That's the whole reason many of us in the .NET community started
recommending vertical slices and feature folders.

It didn't help that the .NET community decided over time that every application
needed a dozen or more projects. We may have gone a little overboard. Some clients
had solutions containing *hundreds* of projects; we've worked to bring those
numbers down to something more sane.

## The duplication myth in "pure" vertical slices

Nick talks about vertical slice architecture as a great alternative to Clean
Architecture, but never shows what his preferred architecture actually looks like.
I looked at the second most popular VSA template on GitHub. Everyone touts that
vertical slices are great because you have everything you need in one folder and
never bounce around. Is that really true?

There's a root-level `features` folder with, say, heroes and teams, and inside an
endpoint you'll find the records, the endpoint, the handler, a validator, and a
summary — all together. Great, the endpoint-specific types are in one file.

But what *isn't* in there? The actual logic — adding the hero, saving changes for
persistence — where does that live? In a `common` folder, which contains all the
usual Clean Architecture folders: domain, persistence, infrastructure. To see how
"add hero" works, you go into the team aggregate where the domain logic lives.

So it's really not that different structurally. In this template, the only thing
centralized *as a feature* are the types needed for an endpoint — not the business
logic, not the persistence, not other infrastructure. If you add a feature that
touches a new domain entity or changes how it's stored, you'll need to bounce
around to those other folders. That's fine — that's just how things work.

**Understand that in most cases, even when people talk about vertical slices,
they're mostly only talking about the UI layer.** Very few developers or
architects suggest you put *all* the business logic and persistence separately in
each feature folder. Not everything belongs duplicated into every feature folder
forever. Some abstractions are useful. Sharing domain concepts instead of
duplicating them is helpful. Reusing consistent infrastructure — like how you
configure your DbContext — makes sense, instead of having a DbContext in every
feature folder.

## The right framing: three separate decisions

This is why the conversation shouldn't be framed as "vertical slices *or* Clean
Architecture," as if they're against each other. That's the wrong framing.
**Feature organization, code reuse, and dependency management are separate aspects
of your architectural choices:**

- **Feature organization** answers *"where do I find the code?"*
- **Code reuse** ensures the logic in your application is consistent between
  features.
- **Dependency management** is about deciding what is allowed to depend on what.

These are all valid things to optimize.

## Why "minimal clean" exists, and enforcing rules without projects

I created the minimal Clean Architecture template because I agreed with a lot of
the criticisms of classic Clean Architecture over the years. Giant multi-repo /
multi-project solutions became cumbersome — too much ceremony, too much project
hopping. I also figured out a way to **still enforce the rules of dependency
management without having separate projects.**

Nick makes the point that, especially with AI, having locality of files helps the
AI discover things. Maybe — they have tools like ripgrep to find anything. But
**consistency really matters for LLM/agent-driven development**: a consistent way
things are organized and behavior is applied matters a lot. And humans naturally
do better when related things are contained together too. Many practices people
are discovering for making AI write better code turn out to be the same things
that were great for human developers.

So minimal clean keeps the benefits I still cared about — dependency inversion,
isolation of infrastructure, proper encapsulation, testability — but is much
simpler, with far less ceremony and fewer projects. With Aspire it's about three
projects, but just one for the application, which makes it simpler to navigate and
allows a more feature-centric organization. There's just one web project, and at
the top you immediately see the features (e.g. cart features, product features),
and inside each you see vertical slices for each endpoint.

## Toward modular monoliths

I was also trying to solve another problem: proper **modules**. Most ASP.NET Core
solutions lack them, partly because the traditional Clean Architecture layout
doesn't map well to real modules — unless you're building microservices, where
each application *is* the module.

But most teams don't need microservices. Many reach for them to solve
*organizational* problems more than technical ones, and many who justify them
technically really just needed **modularity with real boundaries** and couldn't
figure out how to get that without putting a network hop between modules. If you
need real modules, having only `core`, `infrastructure`, and `web` projects
doesn't cut it. Real modules require autonomy, encapsulation, and clearly defined
public contracts. A giant shared core project and a big infrastructure project
provide no boundaries between modules — they exist to fix the *dependency*
problem, which is the point of Clean Architecture, not the *modularity* problem.

So with minimal clean I didn't move away from boundaries — I moved toward
**module boundaries**. These days, for most serious business applications, my
go-to is a **modular monolith**: not distributed microservices, not a giant shared
layered monolith, but a modular system organized around business capabilities
(books, email, orders, reports, user management) at the top level. To work on
reporting, you stay in the reporting module. You get clear contracts, explicit
module boundaries, and strong encapsulation that keeps modules independent — all
still organized around features and built in vertical slices. These ideas
absolutely coexist.

This gets you many of the benefits people wanted from microservices without the
operational tax of a distributed system. The minimal clean template works great as
a single module in this style. You get feature-centric organization top to bottom,
pragmatic boundaries, strong modularity, and less ceremony — and you can scale up
to different teams owning different modules, with fewer accidental distributed
systems.

## Closing

I appreciated Nick's video — he's reacting against a very real problem. But the
answer isn't to throw away architectural boundaries and rules; it's to use
*better* boundaries. Microservices have lost some luster, but the need for
modularity remains. And keeping more of the system in a monorepo makes it much
easier for AI agents to work on effectively — which further argues for modular
monoliths over a microservices-first approach.
